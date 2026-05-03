namespace TradingEngine.Application.Common;

public static class DateTimeExtensions
{
    public static long ToUnixTimeMs(this DateTime dateTime)
    {
        return new DateTimeOffset(dateTime, TimeSpan.Zero).ToUnixTimeMilliseconds();
    }

    public static long? ToUnixTimeMs(this DateTime? dateTime)
    {
        return dateTime.HasValue
            ? new DateTimeOffset(dateTime.Value, TimeSpan.Zero).ToUnixTimeMilliseconds()
            : null;
    }
}
