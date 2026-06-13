using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.DespesaPeriodo;

// ── Command ───────────────────────────────────────────────────────────────────
public record RemoverDespesaPeriodoCommand(Guid Id) : IRequest<bool>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class RemoverDespesaPeriodoCommandHandler : IRequestHandler<RemoverDespesaPeriodoCommand, bool>
{
    private readonly IDespesaPeriodoRepository _repo;

    public RemoverDespesaPeriodoCommandHandler(IDespesaPeriodoRepository repo) => _repo = repo;

    public async Task<bool> Handle(RemoverDespesaPeriodoCommand cmd, CancellationToken ct)
    {
        var item = await _repo.GetByIdAsync(cmd.Id, ct);
        if (item is null) return false;
        await _repo.DeleteAsync(cmd.Id, ct);
        return true;
    }
}
