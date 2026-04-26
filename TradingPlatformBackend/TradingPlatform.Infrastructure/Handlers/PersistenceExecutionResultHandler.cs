using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TradingEngine.Domain.Enums;
using TradingEngine.Infrastructure.Persistence;
using TradingEngine.MatchingEngine.Abstractions;
using TradingEngine.MatchingEngine.Models;
using TradingEngine.Domain.Entities;
using TradingEngine.Domain.ValueObjects;

namespace TradingEngine.Infrastructure.Handlers;

/// <summary>
/// Persists trades and updates balances/positions based on execution results from the matching engine.
/// Lives in Infrastructure to access DbContext without creating project reference cycles.
/// </summary>
public sealed class PersistenceExecutionResultHandler : IExecutionResultHandler
{
    private readonly TradingDbContext _dbContext;
    private readonly ILogger<PersistenceExecutionResultHandler> _logger;

    public PersistenceExecutionResultHandler(
        TradingDbContext dbContext,
        ILogger<PersistenceExecutionResultHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task HandleAsync(ExecutionResult result, CancellationToken cancellationToken)
    {
        if (result is not ExecutionResult.Accepted accepted)
            return;

        await PersistAcceptedAsync(accepted, cancellationToken);
    }

    private async Task PersistAcceptedAsync(ExecutionResult.Accepted accepted, CancellationToken cancellationToken)
    {
        await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var orderIds = accepted.Trades
                .SelectMany(t => new[] { t.BuyOrderId, t.SellOrderId })
                .Concat(accepted.StateChanges.Select(sc => sc.OrderId))
                .Distinct()
                .ToList();

            var orders = await _dbContext.Orders
                .Where(o => orderIds.Contains(o.Id))
                .ToDictionaryAsync(o => o.Id, cancellationToken);

            var userIds = orders.Values.Select(o => o.UserId)
                .Concat(accepted.Trades.SelectMany(t => new[] { t.BuyerId, t.SellerId }))
                .Distinct()
                .ToList();

            var accounts = await _dbContext.UserAccounts
                .Where(a => userIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, cancellationToken);

            var symbols = orders.Values.Select(o => o.Symbol).Distinct().ToList();
            var positions = await _dbContext.Positions
                .Where(p => userIds.Contains(p.UserId) && symbols.Contains(p.Symbol))
                .ToListAsync(cancellationToken);

            PositionDomain? FindPosition(Guid userId, Symbol symbol) =>
                positions.FirstOrDefault(p => p.UserId == userId && p.Symbol == symbol);

            // Trades: move cash and update positions.
            foreach (var trade in accepted.Trades)
            {
                var buyOrder = orders[trade.BuyOrderId];
                var sellOrder = orders[trade.SellOrderId];

                var price = new Price(trade.Price);
                var qty = new Quantity(trade.Quantity);
                var notional = price.Value * qty.Value;
                var currency = accounts[trade.BuyerId].Balance.Currency;
                var money = new Money(notional, currency);

                var buyer = accounts[trade.BuyerId];
                var seller = accounts[trade.SellerId];

                buyer.CommitReservedFunds(money);
                seller.Deposit(money);

                var buyPos = FindPosition(trade.BuyerId, buyOrder.Symbol);
                if (buyPos is null)
                {
                    buyPos = PositionDomain.Create(trade.BuyerId, buyOrder.Symbol, qty, price.Value)
                        ?? throw new UnreachableException();
                    _dbContext.Positions.Add(buyPos);
                    positions.Add(buyPos);
                }
                else
                {
                    buyPos.Add(qty, price.Value);
                }

                var sellPos = FindPosition(trade.SellerId, sellOrder.Symbol)
                              ?? throw new InvalidOperationException("Seller position not found for symbol");
                sellPos.Reduce(qty);

                var executedAt = DateTimeOffset.FromUnixTimeMilliseconds(trade.ExecutedAt).UtcDateTime;
                var tradeDomain = TradeDomain.Create(
                    trade.TradeId,
                    trade.BuyOrderId,
                    trade.SellOrderId,
                    trade.BuyerId,
                    trade.SellerId,
                    buyOrder.Symbol,
                    price,
                    qty,
                    executedAt);
                _dbContext.Trades.Add(tradeDomain);
            }

            // Order states and releasing unused reserves.
            foreach (var stateChange in accepted.StateChanges)
            {
                if (!orders.TryGetValue(stateChange.OrderId, out var order))
                    continue;

                var filledBefore = order.Quantity.Value - order.RemainingQuantity.Value;
                var newlyFilled = stateChange.FilledQuantity - (long)filledBefore;
                if (newlyFilled > 0)
                {
                    order.Fill(new Quantity(newlyFilled));
                }

                if (stateChange.Status == OrderStatus.Cancelled)
                {
                    order.Cancel();
                }

                if (order.Side == OrderSide.Buy && stateChange.Status == OrderStatus.Cancelled && stateChange.RemainingQuantity > 0)
                {
                    var release = new Money(order.Price.Value * stateChange.RemainingQuantity, accounts[order.UserId].Balance.Currency);
                    accounts[order.UserId].ReleaseReservedFunds(release);
                }

                _dbContext.Orders.Update(order);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist execution result");
            await tx.RollbackAsync(cancellationToken);
        }
    }
}
