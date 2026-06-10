using Ledger.Domain.Base;

namespace Ledger.Domain.Interfaces;

/// <summary>
/// Contrato genérico de repositório para qualquer entidade de domínio.
/// </summary>
public interface IRepository<T> where T : BaseDomain
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
