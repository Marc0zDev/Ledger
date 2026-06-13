using AutoMapper;
using Ledger.Application.DTOs.DespesaPeriodo;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.DespesaPeriodo;

// ── Command ───────────────────────────────────────────────────────────────────
public record AnexarBoletoPeriodoCommand(Guid Id, string Path) : IRequest<DespesaPeriodoResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class AnexarBoletoPeriodoCommandHandler : IRequestHandler<AnexarBoletoPeriodoCommand, DespesaPeriodoResponse?>
{
    private readonly IDespesaPeriodoRepository _repo;
    private readonly ICategoriaRepository      _categoriaRepo;
    private readonly IMapper                   _mapper;

    public AnexarBoletoPeriodoCommandHandler(IDespesaPeriodoRepository repo,
        ICategoriaRepository categoriaRepo, IMapper mapper)
    {
        _repo          = repo;
        _categoriaRepo = categoriaRepo;
        _mapper        = mapper;
    }

    public async Task<DespesaPeriodoResponse?> Handle(AnexarBoletoPeriodoCommand cmd, CancellationToken ct)
    {
        var lancamento = await _repo.GetByIdAsync(cmd.Id, ct);
        if (lancamento is null) return null;
        lancamento.AnexarBoleto(cmd.Path);
        await _repo.UpdateAsync(lancamento, ct);

        var categoria = await _categoriaRepo.GetByIdAsync(lancamento.CategoriaId, ct);
        var response  = _mapper.Map<DespesaPeriodoResponse>(lancamento);
        response.CategoriaNome  = categoria?.Nome  ?? string.Empty;
        response.CategoriaIcone = categoria?.Icone;
        response.CategoriaCor   = categoria?.Cor;
        return response;
    }
}
