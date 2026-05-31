using TradingEngine.MatchingEngine.Scaling;

namespace TradingEngine.Application.Features.Orders;

public static class OrderScaleRules
{
    public static bool IsPriceRepresentable(decimal price)
        => EngineScaling.IsPriceRepresentable(price);

    public static bool IsQuantityRepresentable(decimal quantity)
        => EngineScaling.IsQuantityRepresentable(quantity);

    public static decimal PriceStep => 1m / EngineScaling.PriceScale;

    public static decimal QuantityStep => 1m / EngineScaling.QuantityScale;
}
