namespace Ledger.Application.Interfaces;

public interface IIdentityService
{
    /// <summary>Cria usuário via Identity. Retorna (sucesso, erros, userId).</summary>
    Task<(bool Success, IEnumerable<string> Errors, Guid UserId)> CriarUsuarioAsync(
        string nome, string email, string senha, CancellationToken ct = default);

    /// <summary>Verifica e-mail + senha. Retorna true se válido.</summary>
    Task<bool> CheckPasswordAsync(string email, string senha, CancellationToken ct = default);

    /// <summary>Gera token de confirmação de e-mail para o usuário.</summary>
    Task<string> GerarTokenConfirmacaoEmailAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Confirma o e-mail do usuário com o token fornecido.</summary>
    Task<bool> ConfirmarEmailAsync(Guid userId, string token, CancellationToken ct = default);

    /// <summary>Verifica se o e-mail do usuário já foi confirmado.</summary>
    Task<bool> EmailConfirmadoAsync(Guid userId, CancellationToken ct = default);
}
