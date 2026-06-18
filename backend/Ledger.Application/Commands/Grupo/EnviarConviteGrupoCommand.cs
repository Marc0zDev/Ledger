using Ledger.Application.DTOs.Convite;
using Ledger.Application.Interfaces;
using Ledger.Domain.Entities;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Grupo;

// ── Command ───────────────────────────────────────────────────────────────────
public record EnviarConviteGrupoCommand(Guid GrupoId, Guid UsuarioId, Guid SolicitanteId)
    : IRequest<ConviteGrupoResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class EnviarConviteGrupoCommandHandler : IRequestHandler<EnviarConviteGrupoCommand, ConviteGrupoResponse>
{
    private readonly IGrupoRepository        _grupoRepository;
    private readonly IUsuarioRepository      _usuarioRepository;
    private readonly IConviteGrupoRepository _conviteGrupoRepository;
    private readonly IEmailService           _emailService;

    public EnviarConviteGrupoCommandHandler(
        IGrupoRepository grupoRepository,
        IUsuarioRepository usuarioRepository,
        IConviteGrupoRepository conviteGrupoRepository,
        IEmailService emailService)
    {
        _grupoRepository        = grupoRepository;
        _usuarioRepository      = usuarioRepository;
        _conviteGrupoRepository = conviteGrupoRepository;
        _emailService           = emailService;
    }

    public async Task<ConviteGrupoResponse> Handle(EnviarConviteGrupoCommand cmd, CancellationToken ct)
    {
        var grupo = await _grupoRepository.GetComMembrosAsync(cmd.GrupoId, ct)
            ?? throw new DomainValidationException(["Grupo não encontrado."]);

        var solicitante = grupo.Membros.FirstOrDefault(m => m.UsuarioId == cmd.SolicitanteId);
        if (solicitante?.Role != RoleGrupo.Chefe)
            throw new DomainValidationException(["Apenas o chefe do grupo pode convidar membros."]);

        if (grupo.Membros.Any(m => m.UsuarioId == cmd.UsuarioId))
            throw new DomainValidationException(["Este usuário já é membro do grupo."]);

        var jaPendente = await _conviteGrupoRepository.ExistePendenteAsync(cmd.GrupoId, cmd.UsuarioId, ct);
        if (jaPendente)
            throw new DomainValidationException(["Já existe um convite pendente para este usuário."]);

        var usuario = await _usuarioRepository.GetByIdAsync(cmd.UsuarioId, ct)
            ?? throw new DomainValidationException(["Usuário não encontrado."]);

        var convite = ConviteGrupoDomain.Criar(cmd.GrupoId, cmd.SolicitanteId, cmd.UsuarioId);

        if (!convite.IsValid)
            throw new DomainValidationException(convite.Notifications.Select(n => n.Message));

        await _conviteGrupoRepository.AddAsync(convite, ct);

        var link = $"http://localhost:4200/convites/aceitar?token={convite.Token}&tipo=grupo";
        try
        {
            var corpo = Templates.EmailTemplates.ConviteGrupo(
                usuario.Nome, "Ledger", grupo.Nome, link);
            await _emailService.EnviarAsync(
                usuario.Email,
                $"Você foi convidado para o grupo \"{grupo.Nome}\"",
                corpo, ct);
        }
        catch { }

        return new ConviteGrupoResponse
        {
            Id        = convite.Id,
            GrupoId   = convite.GrupoId,
            GrupoNome = grupo.Nome,
            Token     = convite.Token,
            Status    = convite.Status.ToString(),
            ExpiresAt = convite.ExpiresAt,
            CreatedAt = convite.CreatedAt,
        };
    }
}
