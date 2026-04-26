using TradingEngine.Domain.Enums;

namespace TradingEngine.Application.Features.Trades.Dtos;

public class TradeDto
{
    public Guid TradeId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public OrderSide Side { get; set; }
    public long ExecutedAt { get; set; }
}
