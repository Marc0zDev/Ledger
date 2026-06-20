using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Receita;

// ── Command ───────────────────────────────────────────────────────────────────
public record DeletarReceitaCommand(Guid Id) : IRequest<bool>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class DeletarReceitaCommandHandler : IRequestHandler<DeletarReceitaCommand, bool>
{
    private readonly IReceitaRepository _repo;

    public DeletarReceitaCommandHandler(IReceitaRepository repo) => _repo = repo;

    public async Task<bool> Handle(DeletarReceitaCommand cmd, CancellationToken ct)
    {
        var receita = await _repo.GetByIdAsync(cmd.Id, ct);
        if (receita is null) return false;
        await _repo.DeleteAsync(cmd.Id, ct);
        return true;
    }
}
