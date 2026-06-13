using AutoMapper;
using Ledger.Application.DTOs.Despesa;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Despesa;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ObterDespesaQuery(Guid Id) : IRequest<DespesaResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ObterDespesaQueryHandler : IRequestHandler<ObterDespesaQuery, DespesaResponse?>
{
    private readonly IDespesaRepository   _despesaRepo;
    private readonly ICategoriaRepository _categoriaRepo;
    private readonly IMapper              _mapper;

    public ObterDespesaQueryHandler(IDespesaRepository despesaRepo,
        ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _despesaRepo   = despesaRepo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<DespesaResponse?> Handle(ObterDespesaQuery query, CancellationToken ct)
    {
        var despesa = await _despesaRepo.GetByIdAsync(query.Id, ct);
        if (despesa is null) return null;

        var r   = _mapper.Map<DespesaResponse>(despesa);
        var cat = await _categoriaRepo.GetByIdAsync(despesa.CategoriaId, ct);
        if (cat is not null)
        {
            r.CategoriaNome  = cat.Nome;
            r.CategoriaIcone = cat.Icone;
            r.CategoriaCor   = cat.Cor;
        }
        return r;
    }
}
