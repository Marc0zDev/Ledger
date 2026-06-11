using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface IMovimentacaoRepository : IRepository<MovimentacaoDomain>
{
    Task<IEnumerable<MovimentacaoDomain>> GetByCofreIdAsync(Guid cofreId, CancellationToken ct = default);
    Task<(IEnumerable<MovimentacaoDomain> Items, int Total)> GetPagedByCofreIdAsync(
        Guid cofreId, int page, int pageSize, CancellationToken ct = default);
}
