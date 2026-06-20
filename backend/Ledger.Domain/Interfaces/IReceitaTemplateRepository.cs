using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface IReceitaTemplateRepository : IRepository<ReceitaTemplateDomain>
{
    Task<IEnumerable<ReceitaTemplateDomain>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct);
    Task<IEnumerable<ReceitaTemplateDomain>> GetAtivosAsync(Guid usuarioId, CancellationToken ct);
}
