using Ledger.Application.DTOs.Movimentacao;

namespace Ledger.Application.Interfaces;

public interface IMovimentacaoService
{
    Task<IEnumerable<MovimentacaoResponse>> ListarPorCofreAsync(Guid cofreId, CancellationToken ct = default);
    Task<MovimentacaoResponse> RegistrarAsync(Guid cofreId, Guid usuarioId, CriarMovimentacaoRequest request, CancellationToken ct = default);
}
