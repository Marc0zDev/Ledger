using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface IUsuarioRepository : IRepository<UsuarioDomain>
{
    Task<UsuarioDomain?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default);
}
