using System.Linq;
using TradingEngine.Domain.Enums;
using TradingEngine.MatchingEngine.Models;

namespace TradingEngine.MatchingEngine.Models;

/// <summary>
/// Single-symbol order book with price-time priority.
/// </summary>
public class OrderBook
{
    private readonly SortedDictionary<long, PriceLevel> _bidLevels = new(new DescComparer()); // Buy: highest first
    private readonly SortedDictionary<long, PriceLevel> _askLevels = new(); // Sell: lowest first
    private readonly Dictionary<Guid, (EngineOrder Order, OrderSide Side)> _orderMap = new();

    public string Symbol { get; }

    public OrderBook(string symbol)
    {
        Symbol = symbol;
    }

    public void AddOrder(EngineOrder order)
    {
        var levels = order.Side == OrderSide.Buy ? _bidLevels : _askLevels;

        if (!levels.TryGetValue(order.Price, out var level))
        {
            level = new PriceLevel(order.Price);
            levels[order.Price] = level;
        }

        level.AddOrder(order);
        _orderMap[order.Id] = (order, order.Side);
    }

    public bool RemoveOrder(Guid orderId)
    {
        if (!_orderMap.TryGetValue(orderId, out var orderInfo))
            return false;

        var levels = orderInfo.Side == OrderSide.Buy ? _bidLevels : _askLevels;
        var price = orderInfo.Order.Price;

        if (levels.TryGetValue(price, out var level))
        {
            level.RemoveOrder(orderId);

            if (!level.HasOrders)
                levels.Remove(price);
        }

        _orderMap.Remove(orderId);
        return true;
    }

    public IReadOnlyList<EngineOrder> GetBidOrders()
    {
        var result = new List<EngineOrder>();
        foreach (var level in _bidLevels.Values)
        {
            result.AddRange(level.Orders);
        }
        return result.AsReadOnly();
    }

    public IReadOnlyList<EngineOrder> GetAskOrders()
    {
        var result = new List<EngineOrder>();
        foreach (var level in _askLevels.Values)
        {
            result.AddRange(level.Orders);
        }
        return result.AsReadOnly();
    }

    public bool HasOrderById(Guid orderId) => _orderMap.ContainsKey(orderId);

    public EngineOrder? FindOrder(Guid orderId)
    {
        return _orderMap.TryGetValue(orderId, out var orderInfo) ? orderInfo.Order : null;
    }

    public OrderBookSnapshot Snapshot()
    {
        var bids = SnapshotSide(_bidLevels, OrderSide.Buy);
        var asks = SnapshotSide(_askLevels, OrderSide.Sell);
        return new OrderBookSnapshot(Symbol, bids, asks);
    }

    private static IReadOnlyList<PriceLevelSnapshot> SnapshotSide(
        SortedDictionary<long, PriceLevel> levels,
        OrderSide side)
    {
        var snapshots = new List<PriceLevelSnapshot>(levels.Count);

        foreach (var level in levels.Values)
        {
            var orders = level.Orders
                .Select(o => new OrderBookOrderSnapshot(
                    o.Id,
                    o.UserId,
                    o.Price,
                    o.OriginalQuantity,
                    o.FilledQuantity,
                    o.RemainingQuantity,
                    side,
                    o.CreatedAt))
                .ToList();

            snapshots.Add(new PriceLevelSnapshot(
                level.Price,
                level.TotalQuantity,
                orders));
        }

        return snapshots;
    }

    private sealed class DescComparer : IComparer<long>
    {
        public int Compare(long x, long y) => y.CompareTo(x);
    }
}
