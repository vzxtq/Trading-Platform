using TradingEngine.Domain.Enums;

namespace TradingEngine.Domain.ValueObjects;

/// <summary>
/// Represents a monetary value with currency.
/// Immutable value object.
/// </summary>
public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }

    public Currency Currency { get; }

    public Money()
    {
        Currency = Currency.USD;
    }

    public Money(decimal amount, Currency currency = Currency.USD)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));

        Amount = amount;
        Currency = currency;
    }

    public static Money Zero(Currency currency = Currency.USD) => new(0, currency);

    private static void EnsureSameCurrency(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException(
                $"Currency mismatch: {a.Currency} vs {b.Currency}");
    }

    public static Money operator +(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public static bool operator >(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return a.Amount > b.Amount;
    }

    public static bool operator <(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return a.Amount < b.Amount;
    }

    public static bool operator >=(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return a.Amount >= b.Amount;
    }

    public static bool operator <=(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return a.Amount <= b.Amount;
    }

    public bool Equals(Money? other)
    {
        if (other is null)
            return false;

        return Amount == other.Amount &&
               Currency == other.Currency;
    }

    public override bool Equals(object? obj) => Equals(obj as Money);

    public override int GetHashCode()
        => HashCode.Combine(Amount, Currency);

    public override string ToString()
        => $"{Amount:F2} {Currency}";
}