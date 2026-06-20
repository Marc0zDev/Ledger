using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ledger.Application.DTOs.Auth;
using Ledger.Application.Interfaces;
using Ledger.Domain.Entities;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Ledger.Application.Commands.Auth;

// ── Command ───────────────────────────────────────────────────────────────────
public record LoginCommand(string Email, string Senha) : IRequest<AuthResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IIdentityService   _identityService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration    _configuration;

    public LoginCommandHandler(
        IIdentityService identityService,
        IUsuarioRepository usuarioRepository,
        IConfiguration configuration)
    {
        _identityService   = identityService;
        _usuarioRepository = usuarioRepository;
        _configuration     = configuration;
    }

    public async Task<AuthResponse> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var valido = await _identityService.CheckPasswordAsync(cmd.Email, cmd.Senha, ct);
        if (!valido)
            throw new DomainValidationException(["E-mail ou senha inválidos."]);

        var usuario = await _usuarioRepository.GetByEmailAsync(cmd.Email, ct)
            ?? throw new DomainValidationException(["E-mail ou senha inválidos."]);

        if (!await _identityService.EmailConfirmadoAsync(usuario.Id, ct))
            throw new DomainValidationException(["Confirme seu e-mail antes de entrar. Verifique sua caixa de entrada."]);

        return GerarToken(usuario, _configuration);
    }

    internal static AuthResponse GerarToken(UsuarioDomain usuario, IConfiguration configuration)
    {
        var key              = configuration["Jwt:Key"]!;
        var issuer           = configuration["Jwt:Issuer"]!;
        var audience         = configuration["Jwt:Audience"]!;
        var expiresInMinutes = int.Parse(configuration["Jwt:ExpiresInMinutes"] ?? "1440");

        var expiresAt   = DateTime.UtcNow.AddMinutes(expiresInMinutes);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Name,  usuario.Nome),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            expiresAt,
            signingCredentials: credentials);

        return new AuthResponse
        {
            Token     = new JwtSecurityTokenHandler().WriteToken(token),
            Nome      = usuario.Nome,
            Email     = usuario.Email,
            UsuarioId = usuario.Id,
            ExpiresAt = expiresAt,
        };
    }
}
