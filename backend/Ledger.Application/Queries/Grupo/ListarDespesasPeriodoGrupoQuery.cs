using AutoMapper;
using Ledger.Application.DTOs.DespesaPeriodo;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Grupo;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ListarDespesasPeriodoGrupoQuery(Guid GrupoId, Guid UsuarioId, DateTime Competencia)
    : IRequest<IEnumerable<DespesaPeriodoResponse>>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ListarDespesasPeriodoGrupoQueryHandler
    : IRequestHandler<ListarDespesasPeriodoGrupoQuery, IEnumerable<DespesaPeriodoResponse>>
{
    private readonly IGrupoRepository          _grupoRepo;
    private readonly IDespesaPeriodoRepository _periodoRepo;
    private readonly IDespesaRepository        _despesaRepo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IMapper                   _mapper;

    public ListarDespesasPeriodoGrupoQueryHandler(
        IGrupoRepository grupoRepo,
        IDespesaPeriodoRepository periodoRepo,
        IDespesaRepository despesaRepo,
        ICategoriaRepository categoriaRepo,
        IMapper mapper)
    {
        _grupoRepo     = grupoRepo;
        _periodoRepo   = periodoRepo;
        _despesaRepo   = despesaRepo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<IEnumerable<DespesaPeriodoResponse>> Handle(
        ListarDespesasPeriodoGrupoQuery query, CancellationToken ct)
    {
        var grupo = await _grupoRepo.GetComMembrosAsync(query.GrupoId, ct)
            ?? throw new DomainValidationException(["Grupo não encontrado."]);

        if (grupo.Membros.All(m => m.UsuarioId != query.UsuarioId))
            throw new DomainValidationException(["Você não é membro deste grupo."]);

        var nomesPorUsuario = grupo.Membros
            .Where(m => m.Usuario != null)
            .ToDictionary(m => m.UsuarioId, m => m.Usuario!.Nome);

        var lancamentos = await _periodoRepo.GetByGrupoAndCompetenciaAsync(query.GrupoId, query.Competencia, ct);

        var categoriaIds = lancamentos.Select(l => l.CategoriaId).Distinct();
        var categorias = new Dictionary<Guid, Domain.Entities.CategoriaDomain>();
        foreach (var catId in categoriaIds)
        {
            var cat = await _categoriaRepo.GetByIdAsync(catId, ct);
            if (cat != null) categorias[cat.Id] = cat;
        }

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
            if (nomesPorUsuario.TryGetValue(l.UsuarioId, out var nome))
                r.UsuarioNome = nome;
            if (l.DespesaId.HasValue && arquivoPorDespesa.TryGetValue(l.DespesaId.Value, out var arquivoId))
                r.ArquivoId = arquivoId;
            return r;
        });
    }
}
