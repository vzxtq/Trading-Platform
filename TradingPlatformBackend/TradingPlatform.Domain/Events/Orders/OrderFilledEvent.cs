using TradingEngine.Domain.Common;
using TradingEngine.Domain.ValueObjects;

namespace TradingEngine.Domain.Events.Orders;

public class OrderFilledEvent : DomainEvent
{
    public Guid OrderId { get; }
    public Guid UserId { get; }
    public Symbol Symbol { get; }
    public Quantity Quantity { get; }

    public OrderFilledEvent(
        Guid orderId,
        Guid userId,
        Symbol symbol,
        Quantity quantity)
    {
        AggregateId = orderId;

        OrderId = orderId;
        UserId = userId;
        Symbol = symbol;
        Quantity = quantity;
    }
}