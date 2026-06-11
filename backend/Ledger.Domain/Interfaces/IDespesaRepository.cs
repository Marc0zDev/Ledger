using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface IDespesaRepository : IRepository<DespesaDomain>
{
    /// <summary>Lista todas as contas fixas do usuário (ativas e inativas), sem paginação.</summary>
    Task<IEnumerable<DespesaDomain>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default);

    /// <summary>Lista contas fixas paginadas do usuário.</summary>
    Task<(IEnumerable<DespesaDomain> Items, int Total)> GetPagedByUsuarioIdAsync(
        Guid usuarioId, int page, int pageSize, CancellationToken ct = default);

    /// <summary>Lista apenas contas fixas ativas para geração automática de período.</summary>
    Task<IEnumerable<DespesaDomain>> GetAtivosAsync(Guid usuarioId, CancellationToken ct = default);
}
