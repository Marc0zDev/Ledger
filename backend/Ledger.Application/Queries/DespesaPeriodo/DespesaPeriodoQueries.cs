using AutoMapper;
using Ledger.Application.DTOs.DespesaPeriodo;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.DespesaPeriodo;

// ── Listar lançamentos por competência ────────────────────────────────────────
public record ListarDespesasPeriodoQuery(Guid UsuarioId, DateTime Competencia)
    : IRequest<IEnumerable<DespesaPeriodoResponse>>;

public class ListarDespesasPeriodoQueryHandler
    : IRequestHandler<ListarDespesasPeriodoQuery, IEnumerable<DespesaPeriodoResponse>>
{
    private readonly IDespesaPeriodoRepository _repo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IMapper                   _mapper;

    public ListarDespesasPeriodoQueryHandler(IDespesaPeriodoRepository repo,
        ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _repo          = repo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<IEnumerable<DespesaPeriodoResponse>> Handle(
        ListarDespesasPeriodoQuery query, CancellationToken ct)
    {
        var lancamentos = await _repo.GetByCompetenciaAsync(query.UsuarioId, query.Competencia, ct);
        var categorias  = (await _categoriaRepo.GetByUsuarioIdAsync(query.UsuarioId, ct))
                           .ToDictionary(c => c.Id);

        return lancamentos.Select(l =>
        {
            var r = _mapper.Map<DespesaPeriodoResponse>(l);
            if (categorias.TryGetValue(l.CategoriaId, out var cat))
            {
                r.CategoriaNome  = cat.Nome;
                r.CategoriaIcone = cat.Icone;
                r.CategoriaCor   = cat.Cor;
            }
            return r;
        });
    }
}

// ── Obter lançamento por Id ───────────────────────────────────────────────────
public record ObterDespesaPeriodoQuery(Guid Id) : IRequest<DespesaPeriodoResponse?>;

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
