using Ledger.Application.Interfaces;
using Ledger.Application.Templates;
using Ledger.Domain.Events;
using Ledger.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ledger.Application.Events.Handlers;

/// <summary>
/// Reage ao CofreConcluídoEvent enviando e-mail para todos os participantes do cofre.
/// Separado do CofreService — responsabilidade única, sem acoplamento ao fluxo principal.
/// </summary>
public class CofreConcluídoEventHandler
    : INotificationHandler<DomainEventNotification<CofreConcluídoEvent>>
{
    private readonly ICofreRepository        _cofreRepository;
    private readonly IParticipanteRepository _participanteRepository;
    private readonly IUsuarioRepository      _usuarioRepository;
    private readonly IEmailService           _emailService;
    private readonly ILogger<CofreConcluídoEventHandler> _logger;

    public CofreConcluídoEventHandler(
        ICofreRepository cofreRepository,
        IParticipanteRepository participanteRepository,
        IUsuarioRepository usuarioRepository,
        IEmailService emailService,
        ILogger<CofreConcluídoEventHandler> logger)
    {
        _cofreRepository        = cofreRepository;
        _participanteRepository = participanteRepository;
        _usuarioRepository      = usuarioRepository;
        _emailService           = emailService;
        _logger                 = logger;
    }

    public async Task Handle(
        DomainEventNotification<CofreConcluídoEvent> notification,
        CancellationToken ct)
    {
        var ev    = notification.Event;
        var cofre = await _cofreRepository.GetComDetalhesAsync(ev.CofreId, ct);
        if (cofre is null) return;

        var participantes = await _participanteRepository.GetByCofreIdAsync(ev.CofreId, ct);

        foreach (var p in participantes)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(p.UsuarioId, ct);
                if (usuario is null) continue;

                var corpo = EmailTemplates.CofreConcluido(
                    usuario.Nome, cofre.Nome, cofre.Meta, cofre.TotalMovimentado);

                await _emailService.EnviarAsync(
                    usuario.Email,
                    $"O cofre \"{cofre.Nome}\" foi concluído!",
                    corpo, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Falha ao enviar e-mail de conclusão para participante {UsuarioId}", p.UsuarioId);
            }
        }
    }
}
