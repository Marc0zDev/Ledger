using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Participante;

// ── Command ───────────────────────────────────────────────────────────────────
public record RemoverParticipanteCommand(Guid CofreId, Guid ParticipanteId) : IRequest<bool>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class RemoverParticipanteCommandHandler : IRequestHandler<RemoverParticipanteCommand, bool>
{
    private readonly IParticipanteRepository _participanteRepository;

    public RemoverParticipanteCommandHandler(IParticipanteRepository participanteRepository)
    {
        _participanteRepository = participanteRepository;
    }

    public async Task<bool> Handle(RemoverParticipanteCommand cmd, CancellationToken ct)
    {
        var participante = await _participanteRepository
            .GetByCofreIdAndUsuarioIdAsync(cmd.CofreId, cmd.ParticipanteId, ct);
        if (participante is null) return false;

        await _participanteRepository.DeleteAsync(participante.Id, ct);
        return true;
    }
}
