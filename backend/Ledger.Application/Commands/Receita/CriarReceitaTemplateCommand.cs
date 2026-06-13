using AutoMapper;
using Ledger.Application.DTOs.Receita;
using Ledger.Domain.Entities;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Receita;

// ── Command ───────────────────────────────────────────────────────────────────
public record CriarReceitaTemplateCommand(
    Guid      UsuarioId,
    string    Nome,
    decimal   Valor,
    string?   Descricao,
    DateTime  DataInicio,
    DateTime? DataFim = null) : IRequest<ReceitaTemplateResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class CriarReceitaTemplateCommandHandler : IRequestHandler<CriarReceitaTemplateCommand, ReceitaTemplateResponse>
{
    private readonly IReceitaTemplateRepository _templateRepo;
    private readonly IReceitaRepository         _receitaRepo;
    private readonly IMapper                    _mapper;

    public CriarReceitaTemplateCommandHandler(IReceitaTemplateRepository templateRepo,
        IReceitaRepository receitaRepo, IMapper mapper)
    {
        _templateRepo = templateRepo;
        _receitaRepo  = receitaRepo;
        _mapper       = mapper;
    }

    public async Task<ReceitaTemplateResponse> Handle(CriarReceitaTemplateCommand cmd, CancellationToken ct)
    {
        var dataInicio = DateTime.SpecifyKind(cmd.DataInicio, DateTimeKind.Utc);
        var dataFim    = cmd.DataFim.HasValue
            ? DateTime.SpecifyKind(cmd.DataFim.Value, DateTimeKind.Utc)
            : (DateTime?)null;

        var template = ReceitaTemplateDomain.Criar(cmd.Nome, cmd.Valor, cmd.Descricao,
            cmd.UsuarioId, dataInicio, dataFim);

        if (!template.IsValid)
            throw new DomainValidationException(template.Notifications.Select(n => n.Message));

        await _templateRepo.AddAsync(template, ct);
        await GerarReceitasAsync(template, ct);

        return _mapper.Map<ReceitaTemplateResponse>(template);
    }

    private async Task GerarReceitasAsync(ReceitaTemplateDomain template, CancellationToken ct)
    {
        var competencia = new DateTime(template.DataInicio.Year, template.DataInicio.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fim = template.DataFim.HasValue
            ? new DateTime(template.DataFim.Value.Year, template.DataFim.Value.Month, 1, 0, 0, 0, DateTimeKind.Utc)
            : new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        while (competencia <= fim)
        {
            if (!await _receitaRepo.ExisteParaTemplateNoMesAsync(template.Id, competencia, ct))
            {
                var receita = ReceitaDomain.Criar(template.Nome, template.Valor, template.Descricao,
                    null, competencia, template.UsuarioId, template.Id);
                await _receitaRepo.AddAsync(receita, ct);
            }
            competencia = competencia.AddMonths(1);
        }
    }
}
