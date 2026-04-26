namespace TradingEngine.Domain.Events;

/// <summary>
/// Raised when a trade (match between buy and sell orders) has been executed.
/// </summary>
public class TradeExecutedEvent : DomainEvent
{
    public Guid BuyOrderId { get; }
    public Guid SellOrderId { get; }
    public decimal Price { get; }
    public decimal Quantity { get; }

    public TradeExecutedEvent(Guid tradeId, Guid buyOrderId, Guid sellOrderId, decimal price, decimal quantity)
    {
        AggregateId = tradeId;
        BuyOrderId = buyOrderId;
        SellOrderId = sellOrderId;
        Price = price;
        Quantity = quantity;
    }
}