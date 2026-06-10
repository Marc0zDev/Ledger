namespace Ledger.Domain.Events;

/// <summary>Levantado quando um novo cofre é criado.</summary>
public record CofreCriadoEvent(Guid CofreId, Guid CriadoPorUsuarioId) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}
