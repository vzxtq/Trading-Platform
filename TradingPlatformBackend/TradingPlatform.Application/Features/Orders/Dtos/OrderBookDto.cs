using TradingEngine.Application.Features.Orders.Dtos;

namespace TradingEngine.Application.Features.Orders.Dtos;

public class OrderBookDto
{
    public required string Symbol { get; set; }
    public List<OrderDto> BuyOrders { get; set; } = [];
    public List<OrderDto> SellOrders { get; set; } = [];
}
