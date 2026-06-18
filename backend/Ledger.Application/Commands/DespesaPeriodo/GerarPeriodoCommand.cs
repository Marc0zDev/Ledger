using AutoMapper;
using Ledger.Application.DTOs.DespesaPeriodo;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.DespesaPeriodo;

// ── Command ───────────────────────────────────────────────────────────────────
public record GerarPeriodoCommand(Guid UsuarioId, DateTime Competencia) : IRequest<IEnumerable<DespesaPeriodoResponse>>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class GerarPeriodoCommandHandler : IRequestHandler<GerarPeriodoCommand, IEnumerable<DespesaPeriodoResponse>>
{
    private readonly IDespesaRepository        _despesaRepo;
    private readonly IDespesaPeriodoRepository _periodoRepo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IMapper                   _mapper;

    public GerarPeriodoCommandHandler(IDespesaRepository despesaRepo,
        IDespesaPeriodoRepository periodoRepo, ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _despesaRepo   = despesaRepo;
        _periodoRepo   = periodoRepo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<IEnumerable<DespesaPeriodoResponse>> Handle(GerarPeriodoCommand cmd, CancellationToken ct)
    {
        var competencia = new DateTime(cmd.Competencia.Year, cmd.Competencia.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var templates   = await _despesaRepo.GetAtivosAsync(cmd.UsuarioId, ct);
        var categorias  = (await _categoriaRepo.GetByUsuarioIdAsync(cmd.UsuarioId, ct))
                           .ToDictionary(c => c.Id);

        var criados = new List<DespesaPeriodoDomain>();

        foreach (var t in templates)
        {
            if (await _periodoRepo.ExisteParaTemplateNoMesAsync(t.Id, competencia, ct))
                continue;

            var lancamento = DespesaPeriodoDomain.Criar(
                t.Id, t.CategoriaId, t.UsuarioId, t.Nome, t.ValorPlanejado, competencia, t.GrupoId);

            await _periodoRepo.AddAsync(lancamento, ct);
            criados.Add(lancamento);
        }

        return criados.Select(l =>
        {
            var response = _mapper.Map<DespesaPeriodoResponse>(l);
            if (categorias.TryGetValue(l.CategoriaId, out var cat))
            {
                response.CategoriaNome  = cat.Nome;
                response.CategoriaIcone = cat.Icone;
                response.CategoriaCor   = cat.Cor;
            }
            return response;
        });
    }
}
