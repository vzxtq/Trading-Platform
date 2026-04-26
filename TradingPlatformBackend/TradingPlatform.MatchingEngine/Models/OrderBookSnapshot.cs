using TradingEngine.Domain.Enums;

namespace TradingEngine.MatchingEngine.Models;

/// <summary>
/// Immutable snapshot of a single symbol order book at a point in engine time.
/// </summary>
public sealed record OrderBookSnapshot(
    string Symbol,
    IReadOnlyList<PriceLevelSnapshot> Bids,
    IReadOnlyList<PriceLevelSnapshot> Asks);

public sealed record PriceLevelSnapshot(
    long Price,
    long TotalQuantity,
    IReadOnlyList<OrderBookOrderSnapshot> Orders);

public sealed record OrderBookOrderSnapshot(
    Guid OrderId,
    Guid UserId,
    long Price,
    long OriginalQuantity,
    long FilledQuantity,
    long RemainingQuantity,
    OrderSide Side,
    long CreatedAt);
