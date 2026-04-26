using TradingEngine.Domain.Common;
using TradingEngine.Domain.ValueObjects;

namespace TradingEngine.Domain.Entities;

/// <summary>
/// Represents a user's position in a particular stock symbol.
/// Tracks quantity held for a given symbol.
/// </summary>
public class PositionDomain : BaseEntity
{
    public Guid UserId { get; private set; }
    public Symbol Symbol { get; private set; } = null!;
    public Quantity Quantity { get; private set; } = null!;
    public decimal AverageCost { get; private set; }

    private PositionDomain()
    { }

    /// <summary>
    /// Creates a new position or returns null if quantity becomes zero.
    /// </summary>
    public static PositionDomain? Create(Guid userId, Symbol symbol, Quantity quantity, decimal averageCost)
    {
        if (quantity.Value <= 0)
            return null;

        return new PositionDomain
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Symbol = symbol,
            Quantity = quantity,
            AverageCost = averageCost,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Increases the position by adding to quantity.
    /// </summary>
    public void Add(Quantity quantity, decimal price)
    {
        var totalCost = (Quantity.Value * AverageCost) + (quantity.Value * price);
        var totalQuantity = Quantity.Value + quantity.Value;
        Quantity = new Quantity(totalQuantity);
        AverageCost = totalCost / totalQuantity;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Decreases the position by reducing quantity.
    /// </summary>
    public void Reduce(Quantity quantity)
    {
        if (quantity.IsGreaterThan(Quantity))
            throw new InvalidOperationException("Cannot reduce position more than current quantity");

        if (quantity.IsEqual(Quantity))
        {
            Quantity = new Quantity(0); // This would normally trigger removal
        }
        else
        {
            Quantity = Quantity.Subtract(quantity);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the unrealized P&L based on current market price.
    /// </summary>
    public decimal CalculateUnrealizedPnL(Price currentPrice)
    {
        return (currentPrice.Value - AverageCost) * Quantity.Value;
    }
}