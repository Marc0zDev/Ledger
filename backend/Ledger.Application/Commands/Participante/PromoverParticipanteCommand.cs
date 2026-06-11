using AutoMapper;
using Ledger.Application.DTOs.Participante;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Participante;

// ── Command ───────────────────────────────────────────────────────────────────
public record PromoverParticipanteCommand(Guid CofreId, Guid ParticipanteId, RoleParticipante NovoRole)
    : IRequest<ParticipanteResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class PromoverParticipanteCommandHandler : IRequestHandler<PromoverParticipanteCommand, ParticipanteResponse>
{
    private readonly IParticipanteRepository _participanteRepository;
    private readonly IMapper                 _mapper;

    public PromoverParticipanteCommandHandler(IParticipanteRepository participanteRepository, IMapper mapper)
    {
        _participanteRepository = participanteRepository;
        _mapper                 = mapper;
    }

    public async Task<ParticipanteResponse> Handle(PromoverParticipanteCommand cmd, CancellationToken ct)
    {
        var participante = await _participanteRepository.GetByIdAsync(cmd.ParticipanteId, ct)
            ?? throw new DomainValidationException(["Participante não encontrado."]);

        if (participante.CofreId != cmd.CofreId)
            throw new DomainValidationException(["Participante não pertence a este cofre."]);

        if (cmd.NovoRole == RoleParticipante.Admin)
            participante.PromoverAdmin();
        else
            participante.RebaixarContributor();

        await _participanteRepository.UpdateAsync(participante, ct);
        return _mapper.Map<ParticipanteResponse>(participante);
    }
}
