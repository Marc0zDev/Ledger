using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface IDespesaRepository : IRepository<DespesaDomain>
{
    /// <summary>Lista todos os templates do usuário (ativos e inativos).</summary>
    Task<IEnumerable<DespesaDomain>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default);

    /// <summary>Lista apenas templates ativos para geração automática de período.</summary>
    Task<IEnumerable<DespesaDomain>> GetAtivosAsync(Guid usuarioId, CancellationToken ct = default);
}
