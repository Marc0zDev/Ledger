using AutoMapper;
using Ledger.Application.DTOs.DespesaPeriodo;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.DespesaPeriodo;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ListarDespesasPeriodoQuery(Guid UsuarioId, DateTime Competencia)
    : IRequest<IEnumerable<DespesaPeriodoResponse>>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ListarDespesasPeriodoQueryHandler
    : IRequestHandler<ListarDespesasPeriodoQuery, IEnumerable<DespesaPeriodoResponse>>
{
    private readonly IDespesaPeriodoRepository _repo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IDespesaRepository        _despesaRepo;
    private readonly IMapper                   _mapper;

    public ListarDespesasPeriodoQueryHandler(IDespesaPeriodoRepository repo,
        ICategoriaRepository categoriaRepo, IDespesaRepository despesaRepo, IMapper mapper)
    {
        _repo          = repo;
        _categoriaRepo = categoriaRepo;
        _despesaRepo   = despesaRepo;
        _mapper        = mapper;
    }

    public async Task<IEnumerable<DespesaPeriodoResponse>> Handle(
        ListarDespesasPeriodoQuery query, CancellationToken ct)
    {
        var lancamentos = await _repo.GetByCompetenciaAsync(query.UsuarioId, query.Competencia, ct);
        var categorias  = (await _categoriaRepo.GetByUsuarioIdAsync(query.UsuarioId, ct))
                           .ToDictionary(c => c.Id);

        var despesaIds = lancamentos
            .Where(l => l.DespesaId.HasValue)
            .Select(l => l.DespesaId!.Value)
            .Distinct();

        var arquivoPorDespesa = await _despesaRepo.GetArquivoIdsByIdsAsync(despesaIds, ct);

        return lancamentos.Select(l =>
        {
            var r = _mapper.Map<DespesaPeriodoResponse>(l);
            if (categorias.TryGetValue(l.CategoriaId, out var cat))
            {
                r.CategoriaNome  = cat.Nome;
                r.CategoriaIcone = cat.Icone;
                r.CategoriaCor   = cat.Cor;
            }
            if (l.DespesaId.HasValue && arquivoPorDespesa.TryGetValue(l.DespesaId.Value, out var arquivoId))
                r.ArquivoId = arquivoId;
            return r;
        });
    }
}
