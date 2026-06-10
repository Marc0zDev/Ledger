using Ledger.Application.DTOs.Auth;

namespace Ledger.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<AuthResponse> RegistrarAsync(RegistrarRequest request, CancellationToken ct = default);
}
