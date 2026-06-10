using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface IMovimentacaoRepository : IRepository<MovimentacaoDomain>
{
    Task<IEnumerable<MovimentacaoDomain>> GetByCofreIdAsync(Guid cofreId, CancellationToken ct = default);
}
