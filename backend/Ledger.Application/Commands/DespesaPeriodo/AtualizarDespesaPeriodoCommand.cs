using AutoMapper;
using Ledger.Application.DTOs.DespesaPeriodo;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.DespesaPeriodo;

// ── Command ───────────────────────────────────────────────────────────────────
public record AtualizarDespesaPeriodoCommand(
    Guid    Id,
    string  Descricao,
    decimal ValorPlanejado,
    Guid    CategoriaId) : IRequest<DespesaPeriodoResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class AtualizarDespesaPeriodoCommandHandler : IRequestHandler<AtualizarDespesaPeriodoCommand, DespesaPeriodoResponse?>
{
    private readonly IDespesaPeriodoRepository _repo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IMapper                   _mapper;

    public AtualizarDespesaPeriodoCommandHandler(IDespesaPeriodoRepository repo,
        ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _repo          = repo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<DespesaPeriodoResponse?> Handle(AtualizarDespesaPeriodoCommand cmd, CancellationToken ct)
    {
        var lancamento = await _repo.GetByIdAsync(cmd.Id, ct);
        if (lancamento is null) return null;

        var categoria = await _categoriaRepo.GetByIdAsync(cmd.CategoriaId, ct)
            ?? throw new DomainValidationException(["Categoria não encontrada."]);

        lancamento.Atualizar(cmd.Descricao, cmd.ValorPlanejado, cmd.CategoriaId);

        if (!lancamento.IsValid)
            throw new DomainValidationException(lancamento.Notifications.Select(n => n.Message));

        await _repo.UpdateAsync(lancamento, ct);

        var response = _mapper.Map<DespesaPeriodoResponse>(lancamento);
        response.CategoriaNome  = categoria.Nome;
        response.CategoriaIcone = categoria.Icone;
        response.CategoriaCor   = categoria.Cor;
        return response;
    }
}
