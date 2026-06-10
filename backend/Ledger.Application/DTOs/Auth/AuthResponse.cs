namespace Ledger.Application.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public DateTime ExpiresAt { get; set; }
}
