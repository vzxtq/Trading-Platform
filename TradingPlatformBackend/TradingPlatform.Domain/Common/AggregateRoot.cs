using TradingEngine.Domain.Events;

namespace TradingEngine.Domain.Common;

/// <summary>
/// Base class for aggregate roots.
/// Manages domain events that should be persisted and published.
/// </summary>
public abstract class AggregateRoot : BaseEntity
{
    private readonly List<DomainEvent> _domainEvents = new();

    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}