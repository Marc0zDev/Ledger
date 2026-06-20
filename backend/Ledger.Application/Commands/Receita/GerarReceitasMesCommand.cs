using AutoMapper;
using Ledger.Application.DTOs.Receita;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Receita;

// ── Command ───────────────────────────────────────────────────────────────────
public record GerarReceitasMesCommand(Guid UsuarioId, DateTime Competencia) : IRequest<IEnumerable<ReceitaResponse>>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class GerarReceitasMesCommandHandler : IRequestHandler<GerarReceitasMesCommand, IEnumerable<ReceitaResponse>>
{
    private readonly IReceitaTemplateRepository _templateRepo;
    private readonly IReceitaRepository         _receitaRepo;
    private readonly IMapper                    _mapper;

    public GerarReceitasMesCommandHandler(
        IReceitaTemplateRepository templateRepo,
        IReceitaRepository receitaRepo,
        IMapper mapper)
    {
        _templateRepo = templateRepo;
        _receitaRepo  = receitaRepo;
        _mapper       = mapper;
    }

    public async Task<IEnumerable<ReceitaResponse>> Handle(GerarReceitasMesCommand cmd, CancellationToken ct)
    {
        var competencia = new DateTime(cmd.Competencia.Year, cmd.Competencia.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var templates   = await _templateRepo.GetAtivosAsync(cmd.UsuarioId, ct);
        var criadas     = new List<ReceitaDomain>();

        foreach (var t in templates)
        {
            if (await _receitaRepo.ExisteParaTemplateNoMesAsync(t.Id, competencia, ct))
                continue;

            var receita = ReceitaDomain.Criar(t.Nome, t.Valor, t.Descricao, null, competencia, cmd.UsuarioId, t.Id);
            await _receitaRepo.AddAsync(receita, ct);
            criadas.Add(receita);
        }

        return _mapper.Map<IEnumerable<ReceitaResponse>>(criadas);
    }
}
