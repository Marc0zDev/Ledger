using AutoMapper;
using Ledger.Application.DTOs;
using Ledger.Application.DTOs.Despesa;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Despesa;

// ── Listar contas fixas do usuário (paginado) ─────────────────────────────────
public record ListarDespesasQuery(Guid UsuarioId, int Page = 1, int PageSize = 10)
    : IRequest<PagedResult<DespesaResponse>>;

public class ListarDespesasQueryHandler : IRequestHandler<ListarDespesasQuery, PagedResult<DespesaResponse>>
{
    private readonly IDespesaRepository  _despesaRepo;
    private readonly ICategoriaRepository _categoriaRepo;
    private readonly IMapper             _mapper;

    public ListarDespesasQueryHandler(IDespesaRepository despesaRepo,
        ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _despesaRepo   = despesaRepo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<PagedResult<DespesaResponse>> Handle(ListarDespesasQuery query, CancellationToken ct)
    {
        var (despesas, total) = await _despesaRepo.GetPagedByUsuarioIdAsync(
            query.UsuarioId, query.Page, query.PageSize, ct);

        var categorias = (await _categoriaRepo.GetByUsuarioIdAsync(query.UsuarioId, ct))
                          .ToDictionary(c => c.Id);

        var items = despesas.Select(d =>
        {
            var r = _mapper.Map<DespesaResponse>(d);
            if (categorias.TryGetValue(d.CategoriaId, out var cat))
            {
                r.CategoriaNome  = cat.Nome;
                r.CategoriaIcone = cat.Icone;
                r.CategoriaCor   = cat.Cor;
            }
            return r;
        }).ToList();

        return new PagedResult<DespesaResponse>
        {
            Items      = items,
            Page       = query.Page,
            PageSize   = query.PageSize,
            Total      = total,
        };
    }
}

// ── Obter template por Id ─────────────────────────────────────────────────────
public record ObterDespesaQuery(Guid Id) : IRequest<DespesaResponse?>;

public class ObterDespesaQueryHandler : IRequestHandler<ObterDespesaQuery, DespesaResponse?>
{
    private readonly IDespesaRepository  _despesaRepo;
    private readonly ICategoriaRepository _categoriaRepo;
    private readonly IMapper             _mapper;

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

        var r = _mapper.Map<DespesaResponse>(despesa);
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
