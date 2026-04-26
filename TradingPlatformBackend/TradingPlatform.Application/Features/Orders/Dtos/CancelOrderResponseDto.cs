namespace TradingEngine.Application.Features.Orders.Dtos;

public class CancelOrderResponseDto
{
    public Guid OrderId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
