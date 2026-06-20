using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface IReceitaRepository : IRepository<ReceitaDomain>
{
    Task<IEnumerable<ReceitaDomain>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct);
    Task<IEnumerable<ReceitaDomain>> GetByCompetenciaAsync(Guid usuarioId, DateTime competencia, CancellationToken ct);
    Task<bool> ExisteParaTemplateNoMesAsync(Guid templateId, DateTime competencia, CancellationToken ct);
    Task<IEnumerable<ReceitaDomain>> GetByGrupoAndCompetenciaAsync(Guid grupoId, DateTime competencia, CancellationToken ct = default);
}
