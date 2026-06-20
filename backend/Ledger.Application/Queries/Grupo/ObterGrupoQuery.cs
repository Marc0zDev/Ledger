using AutoMapper;
using Ledger.Application.DTOs.Grupo;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Grupo;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ObterGrupoQuery(Guid GrupoId) : IRequest<GrupoResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ObterGrupoQueryHandler : IRequestHandler<ObterGrupoQuery, GrupoResponse?>
{
    private readonly IGrupoRepository _grupoRepository;
    private readonly IMapper          _mapper;

    public ObterGrupoQueryHandler(IGrupoRepository grupoRepository, IMapper mapper)
    {
        _grupoRepository = grupoRepository;
        _mapper          = mapper;
    }

    public async Task<GrupoResponse?> Handle(ObterGrupoQuery query, CancellationToken ct)
    {
        var grupo = await _grupoRepository.GetComMembrosAsync(query.GrupoId, ct);
        return grupo is null ? null : _mapper.Map<GrupoResponse>(grupo);
    }
}
