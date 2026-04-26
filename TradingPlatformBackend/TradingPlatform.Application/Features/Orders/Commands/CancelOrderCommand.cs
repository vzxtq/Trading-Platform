using System.Text.Json.Serialization;
using TradingEngine.Application.Features.Orders.Repositories;
using TradingEngine.Application.Interfaces.Orders;
using TradingEngine.MatchingEngine.Abstractions;
using TradingEngine.MatchingEngine.Commands;
using TradingEngine.Application.Common;
using TradingEngine.Application.Features.Orders.Dtos;
using TradingEngine.Domain.ValueObjects;

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
    private readonly IMatchingEngineQueue _engineQueue;

    public CancelOrderCommandHandler(
        IOrderRepository orderRepository,
        IMatchingEngineQueue engineQueue)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _engineQueue = engineQueue ?? throw new ArgumentNullException(nameof(engineQueue));
    }

    public async Task<Result<CancelOrderResponseDto>> Handle(
        CancelOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result<CancelOrderResponseDto>.Failure("Order not found");
        }

        if (order.UserId != request.UserId)
        {
            return Result<CancelOrderResponseDto>.Failure("Order does not belong to user");
        }

        // Update domain state
        order.Cancel();
        await _orderRepository.UpdateAsync(order, cancellationToken);

        // Notify engine to remove from book
        var cancelCommand = new TradingEngine.MatchingEngine.Commands.CancelOrderCommand
        {
            OrderId = order.Id,
            Symbol = new Symbol(order.Symbol.Value)
        };

        await _engineQueue.EnqueueAsync(cancelCommand, cancellationToken);

        return Result<CancelOrderResponseDto>.Success(new CancelOrderResponseDto
        {
            OrderId = order.Id,
            Success = true,
            Message = "Cancel request accepted"
        });
    }
}
