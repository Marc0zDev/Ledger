using AutoMapper;
using Ledger.Application.DTOs.Receita;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Receita;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ListarReceitaTemplatesQuery(Guid UsuarioId) : IRequest<IEnumerable<ReceitaTemplateResponse>>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ListarReceitaTemplatesQueryHandler : IRequestHandler<ListarReceitaTemplatesQuery, IEnumerable<ReceitaTemplateResponse>>
{
    private readonly IReceitaTemplateRepository _repo;
    private readonly IMapper                    _mapper;

    public ListarReceitaTemplatesQueryHandler(IReceitaTemplateRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReceitaTemplateResponse>> Handle(ListarReceitaTemplatesQuery query, CancellationToken ct)
    {
        var templates = await _repo.GetByUsuarioIdAsync(query.UsuarioId, ct);
        return _mapper.Map<IEnumerable<ReceitaTemplateResponse>>(templates);
    }
}
