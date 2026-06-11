using AutoMapper;
using Ledger.Application.DTOs.Despesa;
using Ledger.Domain.Entities;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Despesa;

// ── Command ───────────────────────────────────────────────────────────────────
public record RegistrarDespesaCommand(
    string     Nome,
    TipoDespesa Tipo,
    decimal    ValorPlanejado,
    Guid       CategoriaId,
    Guid       UsuarioId,
    int?       DiaVencimento = null) : IRequest<DespesaResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class RegistrarDespesaCommandHandler : IRequestHandler<RegistrarDespesaCommand, DespesaResponse>
{
    private readonly IDespesaRepository  _despesaRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IMapper             _mapper;

    public RegistrarDespesaCommandHandler(IDespesaRepository despesaRepository,
        ICategoriaRepository categoriaRepository, IMapper mapper)
    {
        _despesaRepository   = despesaRepository;
        _categoriaRepository = categoriaRepository;
        _mapper              = mapper;
    }

    public async Task<DespesaResponse> Handle(RegistrarDespesaCommand cmd, CancellationToken ct)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(cmd.CategoriaId, ct)
            ?? throw new DomainValidationException(["Categoria não encontrada."]);

        var despesa = DespesaDomain.Criar(cmd.Nome, cmd.Tipo, cmd.ValorPlanejado,
            cmd.CategoriaId, cmd.UsuarioId, cmd.DiaVencimento);

        if (!despesa.IsValid)
            throw new DomainValidationException(despesa.Notifications.Select(n => n.Message));

        await _despesaRepository.AddAsync(despesa, ct);

        var response = _mapper.Map<DespesaResponse>(despesa);
        response.CategoriaNome  = categoria.Nome;
        response.CategoriaIcone = categoria.Icone;
        response.CategoriaCor   = categoria.Cor;
        return response;
    }
}
