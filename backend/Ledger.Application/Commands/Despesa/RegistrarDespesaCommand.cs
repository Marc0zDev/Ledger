using AutoMapper;
using Ledger.Application.DTOs.Despesa;
using Ledger.Domain.Entities;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Despesa;

// -- Command -------------------------------------------------------------------
public record RegistrarDespesaCommand(
    string           Descricao,
    decimal          Valor,
    DateTime         DataVencimento,
    Guid             UsuarioId,
    CategoriaDespesa Categoria  = CategoriaDespesa.Outro,
    bool             Recorrente = false) : IRequest<DespesaResponse>;

// -- Handler -------------------------------------------------------------------
public class RegistrarDespesaCommandHandler : IRequestHandler<RegistrarDespesaCommand, DespesaResponse>
{
    private readonly IDespesaRepository _despesaRepository;
    private readonly IMapper            _mapper;

    public RegistrarDespesaCommandHandler(IDespesaRepository despesaRepository, IMapper mapper)
    {
        _despesaRepository = despesaRepository;
        _mapper            = mapper;
    }

    public async Task<DespesaResponse> Handle(RegistrarDespesaCommand cmd, CancellationToken ct)
    {
        var despesa = DespesaDomain.Criar(
            cmd.Descricao, cmd.Valor, cmd.DataVencimento, cmd.UsuarioId, cmd.Categoria, cmd.Recorrente);

        if (!despesa.IsValid)
            throw new DomainValidationException(despesa.Notifications.Select(n => n.Message));

        await _despesaRepository.AddAsync(despesa, ct);
        return _mapper.Map<DespesaResponse>(despesa);
    }
}
