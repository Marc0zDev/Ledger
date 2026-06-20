using AutoMapper;
using Ledger.Application.DTOs.DespesaPeriodo;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.DespesaPeriodo;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ObterDespesaPeriodoQuery(Guid Id) : IRequest<DespesaPeriodoResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ObterDespesaPeriodoQueryHandler : IRequestHandler<ObterDespesaPeriodoQuery, DespesaPeriodoResponse?>
{
    private readonly IDespesaPeriodoRepository _repo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IMapper                   _mapper;

    public ObterDespesaPeriodoQueryHandler(IDespesaPeriodoRepository repo,
        ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _repo          = repo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<DespesaPeriodoResponse?> Handle(ObterDespesaPeriodoQuery query, CancellationToken ct)
    {
        var l = await _repo.GetByIdAsync(query.Id, ct);
        if (l is null) return null;

        var r   = _mapper.Map<DespesaPeriodoResponse>(l);
        var cat = await _categoriaRepo.GetByIdAsync(l.CategoriaId, ct);
        if (cat is not null)
        {
            r.CategoriaNome  = cat.Nome;
            r.CategoriaIcone = cat.Icone;
            r.CategoriaCor   = cat.Cor;
        }
        return r;
    }
}
