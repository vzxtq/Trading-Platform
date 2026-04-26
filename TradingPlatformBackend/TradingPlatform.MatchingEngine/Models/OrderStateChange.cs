using TradingEngine.Domain.Enums;

namespace TradingEngine.MatchingEngine.Models;

/// <summary>
/// Snapshot of an order after processing a taker command.
/// Quantities are cumulative (total filled/remaining after the operation).
/// </summary>
public sealed record OrderStateChange(
    Guid OrderId,
    Guid UserId,
    long FilledQuantity,
    long RemainingQuantity,
    OrderStatus Status);
