using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface IConviteRepository : IRepository<ConviteDomain>
{
    Task<ConviteDomain?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task<IEnumerable<ConviteDomain>> GetPendentesByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default);
    Task<bool> ExistePendenteAsync(Guid cofreId, Guid usuarioId, CancellationToken ct = default);
}
