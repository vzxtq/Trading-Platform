using TradingEngine.Domain.Enums;

namespace TradingEngine.Application.Features.Orders.Dtos;

public class PlaceOrderResponseDto
{
    public Guid OrderId { get; init; }
    public OrderStatus Status { get; init; }
    public string Message { get; init; } = string.Empty;
}
