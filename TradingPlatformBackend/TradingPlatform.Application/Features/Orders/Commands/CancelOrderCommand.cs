using System.Text.Json.Serialization;
using TradingEngine.Application.Features.Orders.Repositories;
using TradingEngine.Application.Interfaces.Orders;
using TradingEngine.Application.Interfaces.Accounts;
using TradingEngine.MatchingEngine.Interfaces;
using TradingEngine.MatchingEngine.Commands;
using TradingEngine.Application.Common;
using TradingEngine.Application.Features.Orders.Dtos;
using TradingEngine.Domain.ValueObjects;
using TradingEngine.Domain.Enums;
using TradingEngine.Application.Interfaces.Positions;
using TradingEngine.Application.Interfaces;
using TradingEngine.Application.Interfaces.Trades;

namespace TradingEngine.Application.Features.Orders.Commands;

/// <summary>
/// Command to cancel an existing order.
/// </summary>
public class CancelOrderCommand : ICommand<Result<CancelOrderResponseDto>>
{
    public Guid OrderId { get; set; }

    [JsonIgnore]
    public Guid UserId { get; set; }
}

public sealed class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand, Result<CancelOrderResponseDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderReadRepository _orderReadRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly ITradeReadRepository _tradeReadRepository;
    private readonly IMatchingEngineQueue _engineQueue;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(
        IOrderRepository orderRepository,
        IOrderReadRepository orderReadRepository,
        IAccountRepository accountRepository,
        IPositionRepository positionRepository,
        ITradeReadRepository tradeReadRepository,
        IMatchingEngineQueue engineQueue,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _orderReadRepository = orderReadRepository ?? throw new ArgumentNullException(nameof(orderReadRepository));
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _positionRepository = positionRepository ?? throw new ArgumentNullException(nameof(positionRepository));
        _tradeReadRepository = tradeReadRepository ?? throw new ArgumentNullException(nameof(tradeReadRepository));
        _engineQueue = engineQueue ?? throw new ArgumentNullException(nameof(engineQueue));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<CancelOrderResponseDto>> Handle(
        CancelOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _orderReadRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result<CancelOrderResponseDto>.Failure("Order not found");

        if (order.UserId != request.UserId)
            return Result<CancelOrderResponseDto>.Failure("Order does not belong to user");

        if (order.Status == OrderStatus.Filled       ||
            order.Status == OrderStatus.Cancelled    ||
            order.Status == OrderStatus.Rejected     ||
            order.Status == OrderStatus.PartiallyFilledCancelled)
        {
            return Result<CancelOrderResponseDto>.Failure($"Cannot cancel order with status {order.Status}");
        }

        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async ct =>
            {
                // Cancel the order via the domain method — raises OrderCancelledEvent and sets status.
                order.Cancel();

                if (order.Side == OrderSide.Buy)
                {
                    var account = await _accountRepository.GetByIdAsync(order.UserId, ct);
                    if (account is not null)
                    {
                        // For partially-filled orders, only the unfilled portion is still reserved.
                        var totalSpent = await _tradeReadRepository.GetTotalSpentOnBuyOrderAsync(order.Id, ct);
                        var releaseAmount = Math.Max(0m, order.ReservedAmount - totalSpent);

                        if (releaseAmount > 0)
                        {
                            account.ReleaseReservedFunds(new Money(releaseAmount, account.Balance.Currency));
                            await _accountRepository.UpdateAsync(account, ct);
                        }
                    }
                }
                else if (order.Side == OrderSide.Sell && order.RemainingQuantity.Value > 0)
                {
                    var position = await _positionRepository.GetUserPositionForSymbolAsync(
                        order.UserId, order.Symbol.Name, ct);

                    if (position is not null)
                    {
                        position.ReleaseReserved(order.RemainingQuantity);
                        await _positionRepository.UpdateAsync(position, ct);
                    }
                }

                await _orderRepository.UpdateAsync(order, ct);
                await _unitOfWork.CommitAsync(ct);

                return true;
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<CancelOrderResponseDto>.Failure($"Failed to cancel order: {ex.Message}");
        }

        // Notify the engine to remove the order from its in-memory book if it is still there.
        // This is best-effort: if the engine does not find the order it returns Rejected,
        // which is harmless because the DB is already committed above.
        var cancelCommand = new MatchingEngine.Commands.CancelOrderCommand
        {
            OrderId = order.Id,
            Symbol = new Symbol(order.Symbol.Name),
            SymbolId = order.SymbolId
        };
        await _engineQueue.EnqueueAsync(cancelCommand, cancellationToken);

        return Result<CancelOrderResponseDto>.Success(new CancelOrderResponseDto
        {
            OrderId = order.Id,
            Message = "Order cancelled"
        });
    }
}
