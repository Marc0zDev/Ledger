using AutoMapper;
using Ledger.Application.DTOs.Despesa;
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
    int?        DiaVencimento) : IRequest<DespesaResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class AtualizarDespesaCommandHandler : IRequestHandler<AtualizarDespesaCommand, DespesaResponse?>
{
    private readonly IDespesaRepository   _despesaRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IMapper              _mapper;

    public AtualizarDespesaCommandHandler(IDespesaRepository despesaRepository,
        ICategoriaRepository categoriaRepository, IMapper mapper)
    {
        _despesaRepository   = despesaRepository;
        _categoriaRepository = categoriaRepository;
        _mapper              = mapper;
    }

    public async Task<DespesaResponse?> Handle(AtualizarDespesaCommand cmd, CancellationToken ct)
    {
        var despesa = await _despesaRepository.GetByIdAsync(cmd.Id, ct);
        if (despesa is null) return null;

        var categoria = await _categoriaRepository.GetByIdAsync(cmd.CategoriaId, ct)
            ?? throw new DomainValidationException(["Categoria não encontrada."]);

        despesa.Atualizar(cmd.Nome, cmd.Tipo, cmd.ValorPlanejado, cmd.CategoriaId, cmd.DiaVencimento);

        if (!despesa.IsValid)
            throw new DomainValidationException(despesa.Notifications.Select(n => n.Message));

        await _despesaRepository.UpdateAsync(despesa, ct);

        var response = _mapper.Map<DespesaResponse>(despesa);
        response.CategoriaNome  = categoria.Nome;
        response.CategoriaIcone = categoria.Icone;
        response.CategoriaCor   = categoria.Cor;
        return response;
    }
}
