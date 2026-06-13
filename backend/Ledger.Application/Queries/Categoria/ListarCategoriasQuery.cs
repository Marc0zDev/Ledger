using AutoMapper;
using Ledger.Application.DTOs.Categoria;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Categoria;

// ── Listar categorias do usuário (sistema + próprias) ─────────────────────────
public record ListarCategoriasQuery(Guid UsuarioId) : IRequest<IEnumerable<CategoriaResponse>>;

public class ListarCategoriasQueryHandler : IRequestHandler<ListarCategoriasQuery, IEnumerable<CategoriaResponse>>
{
    private readonly ICategoriaRepository _repo;
    private readonly IMapper              _mapper;

    public ListarCategoriasQueryHandler(ICategoriaRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoriaResponse>> Handle(ListarCategoriasQuery query, CancellationToken ct)
    {
        var categorias = await _repo.GetByUsuarioIdAsync(query.UsuarioId, ct);
        return _mapper.Map<IEnumerable<CategoriaResponse>>(categorias);
    }
}
