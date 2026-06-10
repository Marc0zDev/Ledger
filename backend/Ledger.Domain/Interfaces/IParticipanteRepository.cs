using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface IParticipanteRepository : IRepository<ParticipanteDomain>
{
    Task<IEnumerable<ParticipanteDomain>> GetByCofreIdAsync(Guid cofreId, CancellationToken ct = default);
    Task<ParticipanteDomain?> GetByCofreIdAndUsuarioIdAsync(Guid cofreId, Guid usuarioId, CancellationToken ct = default);
}
