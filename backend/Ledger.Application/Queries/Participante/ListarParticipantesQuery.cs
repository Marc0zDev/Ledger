using AutoMapper;
using Ledger.Application.DTOs.Participante;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Participante;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ListarParticipantesQuery(Guid CofreId) : IRequest<IEnumerable<ParticipanteResponse>>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ListarParticipantesQueryHandler : IRequestHandler<ListarParticipantesQuery, IEnumerable<ParticipanteResponse>>
{
    private readonly IParticipanteRepository _participanteRepository;
    private readonly IMapper                 _mapper;

    public ListarParticipantesQueryHandler(IParticipanteRepository participanteRepository, IMapper mapper)
    {
        _participanteRepository = participanteRepository;
        _mapper                 = mapper;
    }

    public async Task<IEnumerable<ParticipanteResponse>> Handle(ListarParticipantesQuery query, CancellationToken ct)
    {
        var participantes = await _participanteRepository.GetByCofreIdAsync(query.CofreId, ct);
        return _mapper.Map<IEnumerable<ParticipanteResponse>>(participantes);
    }
}
