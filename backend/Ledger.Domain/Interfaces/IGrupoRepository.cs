using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface IGrupoRepository : IRepository<GrupoDomain>
{
    Task<IEnumerable<GrupoDomain>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default);
    Task<GrupoDomain?> GetComMembrosAsync(Guid grupoId, CancellationToken ct = default);
    Task AddMembroAsync(GrupoMembroDomain membro, CancellationToken ct = default);
    Task DeleteMembroAsync(Guid membroId, CancellationToken ct = default);
}
