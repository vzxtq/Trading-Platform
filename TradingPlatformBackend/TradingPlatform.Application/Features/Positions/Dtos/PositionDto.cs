namespace TradingEngine.Application.Features.Positions.Dtos;

public class PositionDto
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal UnrealizedPnL { get; set; }
    public long LastUpdated { get; set; }
}
