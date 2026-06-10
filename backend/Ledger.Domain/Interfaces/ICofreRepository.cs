using Ledger.Domain.Entities;

namespace Ledger.Domain.Interfaces;

public interface ICofreRepository : IRepository<CofreDomain>
{
    /// <summary>Retorna o cofre com seus Participantes e Movimentações carregados.</summary>
    Task<CofreDomain?> GetComDetalhesAsync(Guid id, CancellationToken ct = default);

    /// <summary>Retorna cofres criados pelo usuário ou nos quais ele é participante.</summary>
    Task<IEnumerable<CofreDomain>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default);
}
