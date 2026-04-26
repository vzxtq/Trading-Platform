namespace TradingEngine.Domain.Events;

/// <summary>
/// Base class for domain events.
/// Domain events represent important business occurrences that have happened.
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public Guid AggregateId { get; protected set; }
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public Guid EventId { get; } = Guid.NewGuid();
}
