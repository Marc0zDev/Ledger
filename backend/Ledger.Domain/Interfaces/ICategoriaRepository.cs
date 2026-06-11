using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface ICategoriaRepository : IRepository<CategoriaDomain>
{
    /// <summary>Retorna categorias do sistema + categorias criadas pelo usuário.</summary>
    Task<IEnumerable<CategoriaDomain>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default);
}
