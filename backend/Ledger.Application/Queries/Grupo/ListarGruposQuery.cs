using AutoMapper;
using Ledger.Application.DTOs.Grupo;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Grupo;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ListarGruposQuery(Guid UsuarioId) : IRequest<IEnumerable<GrupoResponse>>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ListarGruposQueryHandler : IRequestHandler<ListarGruposQuery, IEnumerable<GrupoResponse>>
{
    private readonly IGrupoRepository _grupoRepository;
    private readonly IMapper          _mapper;

    public ListarGruposQueryHandler(IGrupoRepository grupoRepository, IMapper mapper)
    {
        _grupoRepository = grupoRepository;
        _mapper          = mapper;
    }

    public async Task<IEnumerable<GrupoResponse>> Handle(ListarGruposQuery query, CancellationToken ct)
    {
        var grupos = await _grupoRepository.GetByUsuarioIdAsync(query.UsuarioId, ct);
        return _mapper.Map<IEnumerable<GrupoResponse>>(grupos);
    }
}
