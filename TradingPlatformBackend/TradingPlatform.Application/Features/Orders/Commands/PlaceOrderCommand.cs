using TradingEngine.Application.Features.Orders.Repositories;
using TradingEngine.Application.Interfaces.Orders;
using TradingEngine.Application.Interfaces.Accounts;
using TradingEngine.Domain.Entities;
using TradingEngine.Domain.Enums;
using TradingEngine.MatchingEngine.Interfaces;
using TradingEngine.MatchingEngine.Commands;
using TradingEngine.MatchingEngine.Scaling;
using TradingEngine.Application.Common;
using TradingEngine.Application.Features.Orders.Dtos;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Application.Interfaces;
using TradingEngine.Application.Interfaces.Symbols;
using TradingEngine.Application.Interfaces.Positions;

namespace TradingEngine.Application.Features.Orders.Commands;

/// <summary>
/// Command to place a new order in the system.
/// </summary>
public class PlaceOrderCommand : ICommand<Result<PlaceOrderResponseDto>>
{
    public string Symbol { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public decimal Quantity { get; set; }
    public OrderSide Side { get; set; }
    public OrderType Type { get; set; }
}

public sealed class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand, Result<PlaceOrderResponseDto>>
{
    private readonly IMatchingEngineQueue _queue;
    private readonly IOrderRepository _orderRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IUserResolverService _userResolver;
    private readonly ISymbolReadRepository _symbolRepository;
    private readonly IOrderBookSnapshotProvider _snapshotProvider;

    public PlaceOrderCommandHandler(
        IMatchingEngineQueue queue,
        IOrderRepository orderRepository,
        IAccountRepository accountRepository,
        IPositionRepository positionRepository,
        IUserResolverService userResolver,
        ISymbolReadRepository symbolRepository,
        IOrderBookSnapshotProvider snapshotProvider)
    {
        _queue = queue;
        _orderRepository = orderRepository;
        _accountRepository = accountRepository;
        _positionRepository = positionRepository;
        _userResolver = userResolver;
        _symbolRepository = symbolRepository;
        _snapshotProvider = snapshotProvider;
    }

    public async Task<Result<PlaceOrderResponseDto>> Handle(
        PlaceOrderCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userResolver.GetUserId();
        
        try
        {
            var symbolEntity = await _symbolRepository.GetSymbolByNameAsync(request.Symbol, cancellationToken);
            if (symbolEntity == null)
                return Result<PlaceOrderResponseDto>.Failure("Symbol not found");

            var symbolValue = new Symbol(request.Symbol);
            var price = request.Type == OrderType.Limit ? new Price(request.Price!.Value) : null;
            var quantity = new Quantity(request.Quantity);

            var account = await _accountRepository.GetByIdAsync(userId, cancellationToken);
            if (account is null)
                return Result<PlaceOrderResponseDto>.Failure("Account not found");

            // Reserve funds for buy orders up front.
            decimal domainMaxTotalCost = 0;
            if (request.Side == OrderSide.Buy)
            {
                if (request.Type == OrderType.Limit)
                {
                    domainMaxTotalCost = price!.Value * quantity.Value;
                }
                else // Market order
                {
                    var snapshot = await _snapshotProvider.GetSnapshotAsync(symbolValue, cancellationToken);
                    var asks = snapshot.Asks;
                    long costEstimateEngineUnits = 0;
                    long remainingEngineQuantity = quantity.Value.ToEngineQuantity();
                    
                    foreach (var ask in asks)
                    {
                        var fillQuantity = Math.Min(remainingEngineQuantity, ask.TotalQuantity);
                        costEstimateEngineUnits = checked(costEstimateEngineUnits + (fillQuantity * ask.Price));
                        remainingEngineQuantity -= fillQuantity;
                        if (remainingEngineQuantity == 0) break;
                    }
                    
                    if (remainingEngineQuantity > 0)
                        return Result<PlaceOrderResponseDto>.Failure("Insufficient liquidity for market order");
                        
                    // 5% buffer for slippage without decimal arithmetic
                    var costWithSlippageEngineUnits = checked(costEstimateEngineUnits + costEstimateEngineUnits / 20);
                    domainMaxTotalCost = costWithSlippageEngineUnits.ToDomainNotional();
                }

                var money = new Money(domainMaxTotalCost, account.Balance.Currency);
                account.ReserveFunds(money);
                await _accountRepository.UpdateAsync(account, cancellationToken);
            }
            else if (request.Side == OrderSide.Sell)
            {
                var position = await _positionRepository.GetUserPositionForSymbolAsync(userId, request.Symbol, cancellationToken);
                if (position == null || position.AvailableQuantity.IsLessThan(quantity))
                {
                    return Result<PlaceOrderResponseDto>.Failure("Insufficient position");
                }
                
                position.Reserve(quantity);
                await _positionRepository.UpdateAsync(position, cancellationToken);
            }

            // For Buy orders, reservedAmount is the monetary amount reserved.
            // For Sell orders, reservedAmount is the quantity of shares reserved.
            decimal reservedAmount = request.Side == OrderSide.Buy 
                ? domainMaxTotalCost 
                : quantity.Value;

            var order = OrderDomain.Create(
                userId,
                symbolEntity.Id,
                price,
                quantity,
                request.Side,
                request.Type,
                reservedAmount);

            await _orderRepository.AddAsync(order, cancellationToken);

            var command = new AddOrderCommand
            {
                OrderId = order.Id,
                UserId = userId,
                Symbol = symbolValue,
                SymbolId = symbolEntity.Id,
                Price = request.Type == OrderType.Limit ? price!.Value.ToEnginePrice() : 0,
                Quantity = quantity.Value.ToEngineQuantity(),
                Side = request.Side,
                Type = request.Type,
                MaxTotalCost = domainMaxTotalCost.ToEngineNotional(),
                ReceivedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            await _queue.EnqueueAsync(command, cancellationToken);

            return Result<PlaceOrderResponseDto>.Success(new PlaceOrderResponseDto
            {
                OrderId = order.Id,
                Status = OrderStatus.Open,
                Message = "Order queued for matching"
            });
        }
        catch (Exception ex)
        {
            return Result<PlaceOrderResponseDto>.Failure(
                $"Failed to place order: {ex.Message}");
        }
    }
}
