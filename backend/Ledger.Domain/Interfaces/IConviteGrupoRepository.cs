using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface IConviteGrupoRepository : IRepository<ConviteGrupoDomain>
{
    Task<ConviteGrupoDomain?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task<IEnumerable<ConviteGrupoDomain>> GetPendentesByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default);
    Task<bool> ExistePendenteAsync(Guid grupoId, Guid usuarioId, CancellationToken ct = default);
}
