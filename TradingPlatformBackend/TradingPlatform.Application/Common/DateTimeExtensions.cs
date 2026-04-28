namespace TradingEngine.Application.Common;

public static class DateTimeExtensions
{
    public static long ToUnixTimeMs(this DateTime dateTime)
    {
        return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
    }

    public static long? ToUnixTimeMs(this DateTime? dateTime)
    {
        return dateTime?.ToUnixTimeMilliseconds();
    }
}
