using TradingEngine.Application.Common;
using TradingEngine.Application.Features.Orders.Dtos;
using TradingEngine.Application.Interfaces.Orders;

namespace TradingEngine.Application.Features.Orders.Queries;


public class GetUserOrdersQuery : IQuery<Result<IReadOnlyList<OrderDto>>>
{
    public Guid UserId { get; set; }
}

public sealed class GetUserOrdersQueryHandler : IQueryHandler<GetUserOrdersQuery, Result<IReadOnlyList<OrderDto>>>
{
    private readonly IOrderRepository _orderRepository;

    public GetUserOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<Result<IReadOnlyList<OrderDto>>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetUserOrdersAsync(request.UserId, cancellationToken);

        var dtos = orders
            .Select(order => new OrderDto
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
            })
            .ToList();

        return Result<IReadOnlyList<OrderDto>>.Success(dtos);
    }
}
