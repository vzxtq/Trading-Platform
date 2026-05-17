namespace TradingEngine.Domain.ValueObjects;

public record OrderSummary(
    int TotalOrders,
    int OpenOrders,
    int FilledOrders,
    int CancelledOrders,
    decimal TotalVolume)
{
    public double FillRate => TotalOrders == 0 ? 0 : (double)FilledOrders / TotalOrders * 100;
}
