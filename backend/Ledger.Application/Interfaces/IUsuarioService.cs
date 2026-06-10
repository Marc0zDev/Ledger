using Ledger.Application.DTOs.Usuario;

namespace Ledger.Application.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioResponse>> ListarAsync(CancellationToken ct = default);
    Task<UsuarioResponse?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<UsuarioResponse?> ObterPorEmailAsync(string email, CancellationToken ct = default);
    Task<UsuarioResponse> CriarAsync(CriarUsuarioRequest request, CancellationToken ct = default);
    Task<UsuarioResponse?> AtualizarAsync(Guid id, AtualizarUsuarioRequest request, CancellationToken ct = default);
    Task<bool> DeletarAsync(Guid id, CancellationToken ct = default);
}
