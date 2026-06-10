namespace Ledger.Domain.Events;

/// <summary>Levantado quando um novo participante é adicionado a um cofre.</summary>
public record ParticipanteAdicionadoEvent(Guid CofreId, Guid UsuarioId) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}
