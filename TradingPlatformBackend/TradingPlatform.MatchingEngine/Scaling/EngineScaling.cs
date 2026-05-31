using System;

namespace TradingEngine.MatchingEngine.Scaling;

public static class EngineScaling
{
    public const long PriceScale = 100L;
    public const long QuantityScale = 10_000L;
    public const long NotionalScale = PriceScale * QuantityScale;

    public static bool IsPriceRepresentable(decimal price)
        => IsRepresentable(price, PriceScale);

    public static bool IsQuantityRepresentable(decimal quantity)
        => IsRepresentable(quantity, QuantityScale);

    public static bool IsNotionalRepresentable(decimal notional)
        => IsRepresentable(notional, NotionalScale);

    public static long ToEnginePrice(this decimal price)
    {
        if (!IsPriceRepresentable(price))
            throw new ArgumentException($"Price {price} cannot be represented with PriceScale={PriceScale}");

        return checked((long)(price * PriceScale));
    }

    public static decimal ToDomainPrice(this long price)
        => price / (decimal)PriceScale;

    public static long ToEngineQuantity(this decimal quantity)
    {
        if (!IsQuantityRepresentable(quantity))
            throw new ArgumentException($"Quantity {quantity} cannot be represented with QuantityScale={QuantityScale}");

        return checked((long)(quantity * QuantityScale));
    }

    public static decimal ToDomainQuantity(this long quantity)
        => quantity / (decimal)QuantityScale;

    public static long ToEngineNotional(this decimal notional)
    {
        if (!IsNotionalRepresentable(notional))
            throw new ArgumentException($"Notional {notional} cannot be represented with NotionalScale={NotionalScale}");

        return checked((long)(notional * NotionalScale));
    }

    public static decimal ToDomainNotional(this long notional)
        => notional / (decimal)NotionalScale;

    private static bool IsRepresentable(decimal value, long scale)
    {
        var scaled = value * scale;

        return scaled == decimal.Truncate(scaled)
            && scaled >= long.MinValue
            && scaled <= long.MaxValue;
    }
}
