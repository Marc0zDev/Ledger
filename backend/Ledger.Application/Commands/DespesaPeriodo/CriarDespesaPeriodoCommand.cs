using AutoMapper;
using Ledger.Application.DTOs.DespesaPeriodo;
using Ledger.Domain.Entities;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.DespesaPeriodo;

// ── Command ───────────────────────────────────────────────────────────────────
public record CriarDespesaPeriodoCommand(
    Guid?    DespesaId,
    Guid     CategoriaId,
    Guid     UsuarioId,
    string   Descricao,
    decimal  ValorPlanejado,
    DateTime Competencia,
    Guid?    GrupoId = null) : IRequest<DespesaPeriodoResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class CriarDespesaPeriodoCommandHandler : IRequestHandler<CriarDespesaPeriodoCommand, DespesaPeriodoResponse>
{
    private readonly IDespesaPeriodoRepository _repo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IMapper                   _mapper;

    public CriarDespesaPeriodoCommandHandler(IDespesaPeriodoRepository repo,
        ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _repo          = repo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<DespesaPeriodoResponse> Handle(CriarDespesaPeriodoCommand cmd, CancellationToken ct)
    {
        var categoria = await _categoriaRepo.GetByIdAsync(cmd.CategoriaId, ct)
            ?? throw new DomainValidationException(["Categoria não encontrada."]);

        var competencia = new DateTime(cmd.Competencia.Year, cmd.Competencia.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var lancamento = DespesaPeriodoDomain.Criar(
            cmd.DespesaId, cmd.CategoriaId, cmd.UsuarioId,
            cmd.Descricao, cmd.ValorPlanejado, competencia, cmd.GrupoId);

        if (!lancamento.IsValid)
            throw new DomainValidationException(lancamento.Notifications.Select(n => n.Message));

        await _repo.AddAsync(lancamento, ct);

        var response = _mapper.Map<DespesaPeriodoResponse>(lancamento);
        response.CategoriaNome  = categoria.Nome;
        response.CategoriaIcone = categoria.Icone;
        response.CategoriaCor   = categoria.Cor;
        return response;
    }
}
