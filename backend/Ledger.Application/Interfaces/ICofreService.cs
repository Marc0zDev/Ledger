using Ledger.Application.DTOs.Cofre;

namespace Ledger.Application.Interfaces;

public interface ICofreService
{
    Task<IEnumerable<CofreResponse>> ListarAsync(CancellationToken ct = default);
    Task<CofreResponse?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<CofreResponse?> ObterComDetalhesAsync(Guid id, CancellationToken ct = default);
    Task<CofreResponse> CriarAsync(CriarCofreRequest request, Guid criadoPorUsuarioId, CancellationToken ct = default);
    Task<CofreResponse?> AtualizarAsync(Guid id, AtualizarCofreRequest request, CancellationToken ct = default);
    Task<bool> DeletarAsync(Guid id, CancellationToken ct = default);
    Task<CofreResponse?> ConcluirAsync(Guid id, CancellationToken ct = default);
}
