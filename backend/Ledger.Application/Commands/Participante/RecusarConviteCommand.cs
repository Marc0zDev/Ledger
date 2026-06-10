using Ledger.Application.DTOs.Convite;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Participante;

// ── Command ───────────────────────────────────────────────────────────────────
public record RecusarConviteCommand(string Token, Guid UsuarioId) : IRequest<ConviteResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class RecusarConviteCommandHandler : IRequestHandler<RecusarConviteCommand, ConviteResponse>
{
    private readonly IConviteRepository _conviteRepository;
    private readonly ICofreRepository   _cofreRepository;

    public RecusarConviteCommandHandler(IConviteRepository conviteRepository, ICofreRepository cofreRepository)
    {
        _conviteRepository = conviteRepository;
        _cofreRepository   = cofreRepository;
    }

    public async Task<ConviteResponse> Handle(RecusarConviteCommand cmd, CancellationToken ct)
    {
        var convite = await _conviteRepository.GetByTokenAsync(cmd.Token, ct)
            ?? throw new DomainValidationException(["Convite não encontrado."]);

        if (convite.UsuarioId != cmd.UsuarioId)
            throw new DomainValidationException(["Este convite não pertence ao usuário autenticado."]);

        convite.Recusar();

        if (!convite.IsValid)
            throw new DomainValidationException(convite.Notifications.Select(n => n.Message));

        await _conviteRepository.UpdateAsync(convite, ct);

        var cofre = await _cofreRepository.GetByIdAsync(convite.CofreId, ct);

        return new ConviteResponse
        {
            Id        = convite.Id,
            CofreId   = convite.CofreId,
            CofreNome = cofre?.Nome ?? string.Empty,
            Token     = convite.Token,
            Status    = convite.Status.ToString(),
            ExpiresAt = convite.ExpiresAt,
            CreatedAt = convite.CreatedAt,
        };
    }
}
