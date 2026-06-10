using Ledger.Application.DTOs.Convite;
using Ledger.Application.Events;
using Ledger.Application.Interfaces;
using Ledger.Domain.Entities;
using Ledger.Domain.Events;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Participante;

// ── Command ───────────────────────────────────────────────────────────────────
public record EnviarConviteCommand(Guid CofreId, Guid ConvidadoPorUsuarioId, Guid UsuarioId)
    : IRequest<ConviteResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class EnviarConviteCommandHandler : IRequestHandler<EnviarConviteCommand, ConviteResponse>
{
    private readonly ICofreRepository       _cofreRepository;
    private readonly IUsuarioRepository     _usuarioRepository;
    private readonly IConviteRepository     _conviteRepository;
    private readonly IParticipanteRepository _participanteRepository;
    private readonly IEmailService          _emailService;

    public EnviarConviteCommandHandler(
        ICofreRepository cofreRepository,
        IUsuarioRepository usuarioRepository,
        IConviteRepository conviteRepository,
        IParticipanteRepository participanteRepository,
        IEmailService emailService)
    {
        _cofreRepository        = cofreRepository;
        _usuarioRepository      = usuarioRepository;
        _conviteRepository      = conviteRepository;
        _participanteRepository = participanteRepository;
        _emailService           = emailService;
    }

    public async Task<ConviteResponse> Handle(EnviarConviteCommand cmd, CancellationToken ct)
    {
        var cofre = await _cofreRepository.GetComDetalhesAsync(cmd.CofreId, ct)
            ?? throw new DomainValidationException(["Cofre não encontrado."]);

        if (cofre.CriadoPorUsuarioId == cmd.UsuarioId)
            throw new DomainValidationException(["O criador do cofre já é participante automaticamente."]);

        // Verifica se já é participante
        var jaParticipa = cofre.Participantes.Any(p => p.UsuarioId == cmd.UsuarioId);
        if (jaParticipa)
            throw new DomainValidationException(["Este usuário já é participante do cofre."]);

        // Verifica se já há convite pendente
        var jaPendente = await _conviteRepository.ExistePendenteAsync(cmd.CofreId, cmd.UsuarioId, ct);
        if (jaPendente)
            throw new DomainValidationException(["Já existe um convite pendente para este usuário."]);

        var usuario = await _usuarioRepository.GetByIdAsync(cmd.UsuarioId, ct)
            ?? throw new DomainValidationException(["Usuário não encontrado."]);

        var convite = ConviteDomain.Criar(cmd.CofreId, cmd.ConvidadoPorUsuarioId, cmd.UsuarioId);

        if (!convite.IsValid)
            throw new DomainValidationException(convite.Notifications.Select(n => n.Message));

        await _conviteRepository.AddAsync(convite, ct);

        // Envia e-mail com link de aceite
        var link = $"http://localhost:4200/convites/aceitar?token={convite.Token}";
        try
        {
            var corpo = Ledger.Application.Templates.EmailTemplates.ConviteCofre(
                usuario.Nome, "Ledger", cofre.Nome, link);
            await _emailService.EnviarAsync(
                usuario.Email,
                $"Você foi convidado para o cofre \"{cofre.Nome}\"",
                corpo, ct);
        }
        catch { /* falha no e-mail não impede o fluxo */ }

        return new ConviteResponse
        {
            Id        = convite.Id,
            CofreId   = convite.CofreId,
            CofreNome = cofre.Nome,
            Token     = convite.Token,
            Status    = convite.Status.ToString(),
            ExpiresAt = convite.ExpiresAt,
            CreatedAt = convite.CreatedAt,
        };
    }
}
