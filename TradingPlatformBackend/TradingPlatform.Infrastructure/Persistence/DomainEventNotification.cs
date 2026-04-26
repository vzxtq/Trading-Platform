using MediatR;
using TradingEngine.Domain.Events;

namespace TradingEngine.Infrastructure.Persistence;

/// <summary>
/// A generic wrapper that allows specific Domain Events to be dispatched via MediatR
/// without the Domain layer needing a dependency on MediatR.Contracts.
/// </summary>
internal sealed class DomainEventNotification<TDomainEvent> : INotification 
    where TDomainEvent : IDomainEvent
{
    public TDomainEvent DomainEvent { get; }

    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
}
