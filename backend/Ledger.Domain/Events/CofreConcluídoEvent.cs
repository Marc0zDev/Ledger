using Ledger.Domain.Events;

namespace Ledger.Domain.Events;

/// <summary>Levantado quando um cofre é marcado como Concluído.</summary>
public record CofreConcluídoEvent(Guid CofreId) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}
