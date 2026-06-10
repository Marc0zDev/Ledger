using AutoMapper;
using Ledger.Application.DTOs.Despesa;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Despesa;

// ── PagarDespesa ──────────────────────────────────────────────────────────────
public record PagarDespesaCommand(Guid Id, DateTime? DataPagamento = null) : IRequest<DespesaResponse?>;

public class PagarDespesaCommandHandler : IRequestHandler<PagarDespesaCommand, DespesaResponse?>
{
    private readonly IDespesaRepository _despesaRepository;
    private readonly IMapper            _mapper;

    public PagarDespesaCommandHandler(IDespesaRepository despesaRepository, IMapper mapper)
    {
        _despesaRepository = despesaRepository;
        _mapper            = mapper;
    }

    public async Task<DespesaResponse?> Handle(PagarDespesaCommand cmd, CancellationToken ct)
    {
        var despesa = await _despesaRepository.GetByIdAsync(cmd.Id, ct);
        if (despesa is null) return null;

        despesa.Pagar(cmd.DataPagamento);
        await _despesaRepository.UpdateAsync(despesa, ct);
        return _mapper.Map<DespesaResponse>(despesa);
    }
}

// ── AtualizarDespesa ──────────────────────────────────────────────────────────
public record AtualizarDespesaCommand(
    Guid             Id,
    string           Descricao,
    decimal          Valor,
    DateTime         DataVencimento,
    CategoriaDespesa Categoria,
    bool             Recorrente) : IRequest<DespesaResponse?>;

public class AtualizarDespesaCommandHandler : IRequestHandler<AtualizarDespesaCommand, DespesaResponse?>
{
    private readonly IDespesaRepository _despesaRepository;
    private readonly IMapper            _mapper;

    public AtualizarDespesaCommandHandler(IDespesaRepository despesaRepository, IMapper mapper)
    {
        _despesaRepository = despesaRepository;
        _mapper            = mapper;
    }

    public async Task<DespesaResponse?> Handle(AtualizarDespesaCommand cmd, CancellationToken ct)
    {
        var despesa = await _despesaRepository.GetByIdAsync(cmd.Id, ct);
        if (despesa is null) return null;

        despesa.Atualizar(cmd.Descricao, cmd.Valor, cmd.DataVencimento, cmd.Categoria, cmd.Recorrente);

        if (!despesa.IsValid)
            throw new DomainValidationException(despesa.Notifications.Select(n => n.Message));

        await _despesaRepository.UpdateAsync(despesa, ct);
        return _mapper.Map<DespesaResponse>(despesa);
    }
}

// ── AnexarBoleto ──────────────────────────────────────────────────────────────
public record AnexarBoletoCommand(Guid Id, string Path) : IRequest<DespesaResponse?>;

public class AnexarBoletoCommandHandler : IRequestHandler<AnexarBoletoCommand, DespesaResponse?>
{
    private readonly IDespesaRepository _despesaRepository;
    private readonly IMapper            _mapper;

    public AnexarBoletoCommandHandler(IDespesaRepository despesaRepository, IMapper mapper)
    {
        _despesaRepository = despesaRepository;
        _mapper            = mapper;
    }

    public async Task<DespesaResponse?> Handle(AnexarBoletoCommand cmd, CancellationToken ct)
    {
        var despesa = await _despesaRepository.GetByIdAsync(cmd.Id, ct);
        if (despesa is null) return null;

        despesa.AnexarBoleto(cmd.Path);
        await _despesaRepository.UpdateAsync(despesa, ct);
        return _mapper.Map<DespesaResponse>(despesa);
    }
}

