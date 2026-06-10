using Ledger.Application.Interfaces;
using Ledger.Application.Templates;
using Ledger.Domain.Events;
using Ledger.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ledger.Application.Events.Handlers;

/// <summary>
/// Reage ao ParticipanteAdicionadoEvent enviando e-mail de convite ao usuário.
/// </summary>
public class ParticipanteAdicionadoEventHandler
    : INotificationHandler<DomainEventNotification<ParticipanteAdicionadoEvent>>
{
    private readonly ICofreRepository   _cofreRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService      _emailService;
    private readonly ILogger<ParticipanteAdicionadoEventHandler> _logger;

    public ParticipanteAdicionadoEventHandler(
        ICofreRepository cofreRepository,
        IUsuarioRepository usuarioRepository,
        IEmailService emailService,
        ILogger<ParticipanteAdicionadoEventHandler> logger)
    {
        _cofreRepository   = cofreRepository;
        _usuarioRepository = usuarioRepository;
        _emailService      = emailService;
        _logger            = logger;
    }

    public async Task Handle(
        DomainEventNotification<ParticipanteAdicionadoEvent> notification,
        CancellationToken ct)
    {
        var ev      = notification.Event;
        var cofre   = await _cofreRepository.GetByIdAsync(ev.CofreId, ct);
        var usuario = await _usuarioRepository.GetByIdAsync(ev.UsuarioId, ct);
        if (cofre is null || usuario is null) return;

        try
        {
            var corpo = EmailTemplates.ConviteCofre(
                usuario.Nome, "Ledger", cofre.Nome, "http://localhost:4200/cofres");

            await _emailService.EnviarAsync(
                usuario.Email,
                $"Você foi convidado para o cofre \"{cofre.Nome}\"",
                corpo, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Falha ao enviar e-mail de convite para {Email}", usuario.Email);
        }
    }
}
