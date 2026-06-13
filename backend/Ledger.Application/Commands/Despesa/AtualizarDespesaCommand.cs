using AutoMapper;
using Ledger.Application.DTOs.Despesa;
using Ledger.Domain.Entities;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Despesa;

// ── Command ───────────────────────────────────────────────────────────────────
public record AtualizarDespesaCommand(
    Guid        Id,
    string      Nome,
    TipoDespesa Tipo,
    decimal     ValorPlanejado,
    Guid        CategoriaId,
    DateTime    DataInicio,
    DateTime?   DataFim,
    int?        DiaVencimento) : IRequest<DespesaResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class AtualizarDespesaCommandHandler : IRequestHandler<AtualizarDespesaCommand, DespesaResponse?>
{
    private readonly IDespesaRepository        _despesaRepo;
    private readonly IDespesaPeriodoRepository _periodoRepo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IMapper                   _mapper;

    public AtualizarDespesaCommandHandler(IDespesaRepository despesaRepo,
        IDespesaPeriodoRepository periodoRepo, ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _despesaRepo   = despesaRepo;
        _periodoRepo   = periodoRepo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<DespesaResponse?> Handle(AtualizarDespesaCommand cmd, CancellationToken ct)
    {
        var despesa = await _despesaRepo.GetByIdAsync(cmd.Id, ct);
        if (despesa is null) return null;

        var categoria = await _categoriaRepo.GetByIdAsync(cmd.CategoriaId, ct)
            ?? throw new DomainValidationException(["Categoria não encontrada."]);

        var dataInicio = DateTime.SpecifyKind(cmd.DataInicio, DateTimeKind.Utc);
        var dataFim    = cmd.DataFim.HasValue
            ? DateTime.SpecifyKind(cmd.DataFim.Value, DateTimeKind.Utc)
            : (DateTime?)null;

        despesa.Atualizar(cmd.Nome, cmd.Tipo, cmd.ValorPlanejado, cmd.CategoriaId,
            dataInicio, dataFim, cmd.DiaVencimento);

        if (!despesa.IsValid)
            throw new DomainValidationException(despesa.Notifications.Select(n => n.Message));

        await _despesaRepo.UpdateAsync(despesa, ct);
        await GerarPeriodosFaltantesAsync(despesa, ct);

        var response = _mapper.Map<DespesaResponse>(despesa);
        response.CategoriaNome  = categoria.Nome;
        response.CategoriaIcone = categoria.Icone;
        response.CategoriaCor   = categoria.Cor;
        return response;
    }

    private async Task GerarPeriodosFaltantesAsync(DespesaDomain despesa, CancellationToken ct)
    {
        var competencia = new DateTime(despesa.DataInicio.Year, despesa.DataInicio.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fim = despesa.DataFim.HasValue
            ? new DateTime(despesa.DataFim.Value.Year, despesa.DataFim.Value.Month, 1, 0, 0, 0, DateTimeKind.Utc)
            : new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        while (competencia <= fim)
        {
            if (!await _periodoRepo.ExisteParaTemplateNoMesAsync(despesa.Id, competencia, ct))
            {
                var periodo = DespesaPeriodoDomain.Criar(
                    despesa.Id, despesa.CategoriaId, despesa.UsuarioId,
                    despesa.Nome, despesa.ValorPlanejado, competencia);
                await _periodoRepo.AddAsync(periodo, ct);
            }
            competencia = competencia.AddMonths(1);
        }
    }
}
