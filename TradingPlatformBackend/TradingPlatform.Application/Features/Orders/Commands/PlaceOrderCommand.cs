using TradingEngine.Application.Features.Orders.Repositories;
using TradingEngine.Application.Interfaces.Orders;
using TradingEngine.Application.Interfaces.Accounts;
using TradingEngine.Domain.Entities;
using TradingEngine.Domain.Enums;
using TradingEngine.MatchingEngine.Abstractions;
using TradingEngine.MatchingEngine.Commands;
using TradingEngine.Application.Common;
using TradingEngine.Application.Features.Orders.Dtos;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Application.Interfaces;

namespace TradingEngine.Application.Features.Orders.Commands;

/// <summary>
/// Command to place a new order in the system.
/// </summary>
public class PlaceOrderCommand : ICommand<Result<PlaceOrderResponseDto>>
{
    public string Symbol { get; set; } = string.Empty;
    public long Price { get; set; }
    public long Quantity { get; set; }
    public OrderSide Side { get; set; }
}

public sealed class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand, Result<PlaceOrderResponseDto>>
{
    private readonly IMatchingEngineQueue _queue;
    private readonly IOrderRepository _orderRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUserResolverService _userResolver;

    public PlaceOrderCommandHandler(
        IMatchingEngineQueue queue,
        IOrderRepository orderRepository,
        IAccountRepository accountRepository,
        IUserResolverService userResolver)
    {
        _queue = queue;
        _orderRepository = orderRepository;
        _accountRepository = accountRepository;
        _userResolver = userResolver;
    }

    public async Task<Result<PlaceOrderResponseDto>> Handle(
        PlaceOrderCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userResolver.GetUserId();
        var orderId = Guid.NewGuid();

        try
        {
            var symbol = new Symbol(request.Symbol);
            var price = new Price(request.Price);
            var quantity = new Quantity(request.Quantity);

            var account = await _accountRepository.GetByIdAsync(userId, cancellationToken);
            if (account is null)
                return Result<PlaceOrderResponseDto>.Failure("Account not found");

            // Reserve funds for buy orders up front.
            if (request.Side == OrderSide.Buy)
            {
                var notional = price.Value * quantity.Value;
                var money = new Money(notional, account.Balance.Currency);
                account.ReserveFunds(money);
                await _accountRepository.UpdateAsync(account, cancellationToken);
            }

            var order = OrderDomain.Create(
                userId,
                symbol,
                price,
                quantity,
                request.Side);

            await _orderRepository.AddAsync(order, cancellationToken);

            var command = new AddOrderCommand
            {
                OrderId = order.Id,
                UserId = userId,
                Symbol = symbol,
                Price = request.Price,
                Quantity = request.Quantity,
                Side = request.Side,
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
