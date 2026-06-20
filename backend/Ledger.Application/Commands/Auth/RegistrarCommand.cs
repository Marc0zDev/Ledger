using Ledger.Application.DTOs.Auth;
using Ledger.Application.Interfaces;
using Ledger.Application.Templates;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ledger.Application.Commands.Auth;

// ── Command ───────────────────────────────────────────────────────────────────
public record RegistrarCommand(string Nome, string Email, string Senha) : IRequest<AuthResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class RegistrarCommandHandler : IRequestHandler<RegistrarCommand, AuthResponse>
{
    private readonly IIdentityService   _identityService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService      _emailService;
    private readonly IConfiguration    _configuration;
    private readonly ILogger<RegistrarCommandHandler> _logger;

    public RegistrarCommandHandler(
        IIdentityService identityService,
        IUsuarioRepository usuarioRepository,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<RegistrarCommandHandler> logger)
    {
        _identityService   = identityService;
        _usuarioRepository = usuarioRepository;
        _emailService      = emailService;
        _configuration     = configuration;
        _logger            = logger;
    }

    public async Task<AuthResponse> Handle(RegistrarCommand cmd, CancellationToken ct)
    {
        var (success, errors, userId) = await _identityService.CriarUsuarioAsync(cmd.Nome, cmd.Email, cmd.Senha, ct);

        if (!success)
            throw new DomainValidationException(errors);

        var usuario = await _usuarioRepository.GetByIdAsync(userId, ct)
            ?? throw new DomainValidationException(["Erro ao recuperar usuário criado."]);

        try
        {
            var token   = await _identityService.GerarTokenConfirmacaoEmailAsync(userId, ct);
            var baseUrl = _configuration["App:BaseUrl"] ?? "http://localhost:5085";
            var link    = $"{baseUrl}/api/auth/confirmar-email?userId={userId}&token={Uri.EscapeDataString(token)}";
            var corpo   = EmailTemplates.ConfirmacaoConta(cmd.Nome, link);
            await _emailService.EnviarAsync(cmd.Email, "Confirme sua conta no Ledger", corpo, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao enviar e-mail de confirmação para {Email}", cmd.Email);
        }

        return LoginCommandHandler.GerarToken(usuario, _configuration);
    }
}
