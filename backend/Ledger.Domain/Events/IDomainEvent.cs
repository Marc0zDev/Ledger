namespace Ledger.Domain.Events;

/// <summary>
/// Marcador para todos os Domain Events.
/// O Domain não depende de MediatR — o dispatch é feito na camada Application.
/// </summary>
public interface IDomainEvent
{
    DateTime OcorridoEm { get; }
}
