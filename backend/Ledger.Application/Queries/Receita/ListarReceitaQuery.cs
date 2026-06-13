using AutoMapper;
using Ledger.Application.DTOs.Receita;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Receita;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ListarReceitaQuery(Guid UsuarioId, DateTime? Competencia = null) : IRequest<IEnumerable<ReceitaResponse>>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ListarReceitaQueryHandler : IRequestHandler<ListarReceitaQuery, IEnumerable<ReceitaResponse>>
{
    private readonly IReceitaRepository _repo;
    private readonly IMapper            _mapper;

    public ListarReceitaQueryHandler(IReceitaRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReceitaResponse>> Handle(ListarReceitaQuery query, CancellationToken ct)
    {
        if (query.Competencia.HasValue)
        {
            var competencia = new DateTime(query.Competencia.Value.Year, query.Competencia.Value.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var receitas = await _repo.GetByCompetenciaAsync(query.UsuarioId, competencia, ct);
            return _mapper.Map<IEnumerable<ReceitaResponse>>(receitas);
        }
        else
        {
            var receitas = await _repo.GetByUsuarioIdAsync(query.UsuarioId, ct);
            return _mapper.Map<IEnumerable<ReceitaResponse>>(receitas);
        }
    }
}
