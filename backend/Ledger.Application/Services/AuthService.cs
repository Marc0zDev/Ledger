using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ledger.Application.DTOs.Auth;
using Ledger.Application.Interfaces;
using Ledger.Application.Templates;
using Ledger.Domain.Entities;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Ledger.Application.Services;

public class AuthService : IAuthService
{
    private readonly IIdentityService _identityService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IIdentityService identityService,
        IUsuarioRepository usuarioRepository,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _identityService   = identityService;
        _usuarioRepository = usuarioRepository;
        _emailService      = emailService;
        _configuration     = configuration;
        _logger            = logger;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var valido = await _identityService.CheckPasswordAsync(request.Email, request.Senha, ct);
        if (!valido)
            throw new DomainValidationException(["E-mail ou senha inválidos."]);

        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email, ct)
            ?? throw new DomainValidationException(["E-mail ou senha inválidos."]);

        if (!await _identityService.EmailConfirmadoAsync(usuario.Id, ct))
            throw new DomainValidationException(["Confirme seu e-mail antes de entrar. Verifique sua caixa de entrada."]);

        return GerarToken(usuario);
    }

    public async Task<AuthResponse> RegistrarAsync(RegistrarRequest request, CancellationToken ct = default)
    {
        var (success, errors, userId) = await _identityService.CriarUsuarioAsync(
            request.Nome, request.Email, request.Senha, ct);

        if (!success)
            throw new DomainValidationException(errors);

        var usuario = await _usuarioRepository.GetByIdAsync(userId, ct)
            ?? throw new DomainValidationException(["Erro ao recuperar usuário criado."]);

        // Enviar e-mail de confirmação (não bloqueante — falha silenciosa em dev)
        try
        {
            var token   = await _identityService.GerarTokenConfirmacaoEmailAsync(userId, ct);
            var baseUrl = _configuration["App:BaseUrl"] ?? "http://localhost:5085";
            var link    = $"{baseUrl}/api/auth/confirmar-email?userId={userId}&token={Uri.EscapeDataString(token)}";
            var corpo   = EmailTemplates.ConfirmacaoConta(request.Nome, link);
            await _emailService.EnviarAsync(request.Email, "Confirme sua conta no Ledger", corpo, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao enviar e-mail de confirmação para {Email}", request.Email);
        }

        return GerarToken(usuario);
    }

    private AuthResponse GerarToken(UsuarioDomain usuario)
    {
        var key              = _configuration["Jwt:Key"]!;
        var issuer           = _configuration["Jwt:Issuer"]!;
        var audience         = _configuration["Jwt:Audience"]!;
        var expiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "1440");

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
            issuer:            issuer,
            audience:          audience,
            claims:            claims,
            expires:           expiresAt,
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

