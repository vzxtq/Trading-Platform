namespace TradingEngine.Domain.Enums
{
    public enum OrderStatus
    {
        Open = 1,  // Accepted by engine, resting in order book
        PartiallyFilled = 2,  // Partially matched, remainder still in book
        Filled = 3,  // Fully matched, removed from book
        Cancelled = 4,  // Cancelled by user or system
        Rejected = 5   // Rejected by matching engine (validation failure)
    }
}
