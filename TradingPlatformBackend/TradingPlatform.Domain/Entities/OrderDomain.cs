using TradingEngine.Domain.Enums;
using TradingEngine.Domain.Common;
using TradingEngine.Domain.Events;
using TradingEngine.Domain.Events.Orders;
using TradingEngine.Domain.ValueObjects;

namespace TradingEngine.Domain.Entities
{
    public class OrderDomain : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public Symbol Symbol { get; private set; } = null!;
        public Price Price { get; private set; } = null!;
        public Quantity Quantity { get; private set; } = null!;
        public Quantity RemainingQuantity { get; private set; } = null!;

        public OrderSide Side { get; private set; }
        public OrderStatus Status { get; private set; }

        private OrderDomain() { }

        public static OrderDomain Create(
            Guid userId,
            Symbol symbol,
            Price price,
            Quantity quantity,
            OrderSide side)
        {
            var order = new OrderDomain
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Symbol = symbol,
                Price = price,
                Quantity = quantity,
                RemainingQuantity = quantity,
                Side = side,
                Status = OrderStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            order.RaiseDomainEvent(
                new OrderPlacedEvent(order.Id, userId, symbol, price, quantity, side));

            return order;
        }

        public void Fill(Quantity filledQuantity)
        {
            if (filledQuantity.IsGreaterThan(RemainingQuantity))
                throw new InvalidOperationException("Fill exceeds remaining quantity");

            RemainingQuantity = RemainingQuantity.Subtract(filledQuantity);

            if (RemainingQuantity.Value == 0)
            {
                Status = OrderStatus.Filled;

                RaiseDomainEvent(
                    new OrderFilledEvent(
                        Id,
                        UserId,
                        Symbol,
                        Quantity));
            }
            else
            {
                Status = OrderStatus.PartiallyFilled;
            }

            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Filled ||
                Status == OrderStatus.Cancelled ||
                Status == OrderStatus.Rejected)
            {
                throw new InvalidOperationException(
                    $"Cannot cancel order with status {Status}");
            }

            Status = OrderStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;

            RaiseDomainEvent(new OrderCancelledEvent(Id));
        }
    }
}