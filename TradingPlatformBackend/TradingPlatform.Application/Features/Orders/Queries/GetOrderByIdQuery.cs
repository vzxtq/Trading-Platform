using TradingEngine.Application.Common;
using TradingEngine.Application.Features.Orders.Dtos;
using TradingEngine.Application.Interfaces.Orders;

namespace TradingEngine.Application.Features.Orders.Queries;

/// <summary>
/// Query to retrieve an order by its ID.
/// </summary>
public class GetOrderByIdQuery : IQuery<Result<OrderDto>>
{
   public Guid OrderId { get; set; }
}

public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null) return Result<OrderDto>.Failure("Order not found");

        return Result<OrderDto>.Success(new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Symbol = order.Symbol.Value,
            Price = order.Price.Value,
            Quantity = order.Quantity.Value,
            RemainingQuantity = order.RemainingQuantity.Value,
            Side = order.Side,
            Status = order.Status,
            CreatedAt = new DateTimeOffset(order.CreatedAt).ToUnixTimeMilliseconds(),
            UpdatedAt = order.UpdatedAt.HasValue 
                ? new DateTimeOffset(order.UpdatedAt.Value).ToUnixTimeMilliseconds() 
                : null
        });
    }
}
