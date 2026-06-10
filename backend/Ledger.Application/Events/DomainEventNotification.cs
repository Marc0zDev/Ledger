using Ledger.Domain.Events;
using MediatR;

namespace Ledger.Application.Events;

/// <summary>
/// Wrapper que adapta um IDomainEvent para INotification do MediatR.
/// Mantém o Domain sem dependência direta do MediatR.
/// </summary>
public record DomainEventNotification<TEvent>(TEvent Event) : INotification
    where TEvent : IDomainEvent;
