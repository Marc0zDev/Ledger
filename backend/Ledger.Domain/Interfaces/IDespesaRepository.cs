using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface IDespesaRepository : IRepository<DespesaDomain>
{
    Task<IEnumerable<DespesaDomain>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default);
    Task<IEnumerable<DespesaDomain>> GetPendentesAsync(Guid usuarioId, DateTime? vencimentoAte = null, CancellationToken ct = default);
}
