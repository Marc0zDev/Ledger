using Ledger.Application.DTOs.Convite;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Grupo;

// ── Command ───────────────────────────────────────────────────────────────────
public record RecusarConviteGrupoCommand(string Token, Guid UsuarioId) : IRequest<ConviteGrupoResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class RecusarConviteGrupoCommandHandler : IRequestHandler<RecusarConviteGrupoCommand, ConviteGrupoResponse>
{
    private readonly IConviteGrupoRepository _conviteGrupoRepository;
    private readonly IGrupoRepository        _grupoRepository;

    public RecusarConviteGrupoCommandHandler(
        IConviteGrupoRepository conviteGrupoRepository,
        IGrupoRepository grupoRepository)
    {
        _conviteGrupoRepository = conviteGrupoRepository;
        _grupoRepository        = grupoRepository;
    }

    public async Task<ConviteGrupoResponse> Handle(RecusarConviteGrupoCommand cmd, CancellationToken ct)
    {
        var convite = await _conviteGrupoRepository.GetByTokenAsync(cmd.Token, ct)
            ?? throw new DomainValidationException(["Convite não encontrado."]);

        if (convite.UsuarioId != cmd.UsuarioId)
            throw new DomainValidationException(["Este convite não pertence ao usuário autenticado."]);

        convite.Recusar();

        if (!convite.IsValid)
            throw new DomainValidationException(convite.Notifications.Select(n => n.Message));

        await _conviteGrupoRepository.UpdateAsync(convite, ct);

        var grupo = await _grupoRepository.GetByIdAsync(convite.GrupoId, ct);

        return new ConviteGrupoResponse
        {
            Id        = convite.Id,
            GrupoId   = convite.GrupoId,
            GrupoNome = grupo?.Nome ?? string.Empty,
            Token     = convite.Token,
            Status    = convite.Status.ToString(),
            ExpiresAt = convite.ExpiresAt,
            CreatedAt = convite.CreatedAt,
        };
    }
}
