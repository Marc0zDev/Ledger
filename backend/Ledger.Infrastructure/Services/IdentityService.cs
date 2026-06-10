using Ledger.Application.Interfaces;
using Ledger.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Ledger.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Success, IEnumerable<string> Errors, Guid UserId)> CriarUsuarioAsync(
        string nome, string email, string senha, CancellationToken ct = default)
    {
        var user = new ApplicationUser
        {
            Id        = Guid.NewGuid(),
            Nome      = nome,
            UserName  = email,
            Email     = email,
            CreatedAt = DateTime.UtcNow,
        };

        var result = await _userManager.CreateAsync(user, senha);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description), Guid.Empty);

        return (true, [], user.Id);
    }

    public async Task<bool> CheckPasswordAsync(string email, string senha, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return false;
        return await _userManager.CheckPasswordAsync(user, senha);
    }

    public async Task<string> GerarTokenConfirmacaoEmailAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new InvalidOperationException("Usuário não encontrado.");
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<bool> ConfirmarEmailAsync(Guid userId, string token, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return false;
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<bool> EmailConfirmadoAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user?.EmailConfirmed ?? false;
    }
}
