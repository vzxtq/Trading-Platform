namespace TradingEngine.MatchingEngine.Models;

public sealed class PriceLevel
{
    private readonly LinkedList<EngineOrder> _orders = new();
    private readonly Dictionary<Guid, LinkedListNode<EngineOrder>> _nodeMap = new();

    public long Price { get; }
    public bool HasOrders => _orders.Count > 0;
    public IEnumerable<EngineOrder> Orders => _orders;

    public long TotalQuantity => _orders.Sum(o => o.RemainingQuantity);

    public PriceLevel(long price)
    {
        if (price <= 0) throw new ArgumentOutOfRangeException(nameof(price));
        Price = price;
    }

    public void AddOrder(EngineOrder order)
    {
        var node = _orders.AddLast(order);
        _nodeMap[order.Id] = node;
    }

    public bool RemoveOrder(Guid orderId)
    {
        if (!_nodeMap.TryGetValue(orderId, out var node))
            return false;

        _orders.Remove(node);
        _nodeMap.Remove(orderId);
        return true;
    }

    public EngineOrder? PeekOrder() => _orders.First?.Value;
}