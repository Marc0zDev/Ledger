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

    /// <summary>Verifica se o arquivo pertence a uma despesa do usuário.</summary>
    Task<bool> UsuarioPossuiArquivoAsync(Guid arquivoId, Guid usuarioId, CancellationToken ct = default);

    /// <summary>Retorna ArquivoId por template de despesa.</summary>
    Task<Dictionary<Guid, Guid?>> GetArquivoIdsByIdsAsync(IEnumerable<Guid> despesaIds, CancellationToken ct = default);
}
