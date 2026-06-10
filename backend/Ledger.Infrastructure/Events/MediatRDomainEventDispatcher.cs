using Ledger.Application.Events;
using Ledger.Domain.Base;
using Ledger.Domain.Events;
using MediatR;

namespace Ledger.Infrastructure.Events;

/// <summary>
/// Despacha domain events para o MediatR usando reflexão para manter o Domain sem
/// dependência direta do MediatR. Cria dinamicamente um DomainEventNotification[TEvent]
/// para cada IDomainEvent coletado na entidade e publica via IPublisher.
/// </summary>
public class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public MediatRDomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchAsync(BaseDomain entity, CancellationToken ct = default)
    {
        var events = entity.DomainEvents.ToList();
        entity.ClearDomainEvents();

        foreach (var domainEvent in events)
            await PublishAsync(domainEvent, ct);
    }

    public async Task DispatchAsync(IEnumerable<BaseDomain> entities, CancellationToken ct = default)
    {
        var allEvents = entities
            .SelectMany(e =>
            {
                var evs = e.DomainEvents.ToList();
                e.ClearDomainEvents();
                return evs;
            })
            .ToList();

        foreach (var domainEvent in allEvents)
            await PublishAsync(domainEvent, ct);
    }

    private async Task PublishAsync(IDomainEvent domainEvent, CancellationToken ct)
    {
        // Cria DomainEventNotification<TConcreteEvent> via reflexão
        var notificationType = typeof(DomainEventNotification<>)
            .MakeGenericType(domainEvent.GetType());

        var notification = Activator.CreateInstance(notificationType, domainEvent);
        if (notification is INotification mediatRNotification)
            await _publisher.Publish(mediatRNotification, ct);
    }
}
