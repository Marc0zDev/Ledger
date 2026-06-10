using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Despesa;

// ── Command ───────────────────────────────────────────────────────────────────
public record RemoverDespesaCommand(Guid Id) : IRequest<bool>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class RemoverDespesaCommandHandler : IRequestHandler<RemoverDespesaCommand, bool>
{
    private readonly IDespesaRepository _despesaRepository;

    public RemoverDespesaCommandHandler(IDespesaRepository despesaRepository)
    {
        _despesaRepository = despesaRepository;
    }

    public async Task<bool> Handle(RemoverDespesaCommand cmd, CancellationToken ct)
    {
        var despesa = await _despesaRepository.GetByIdAsync(cmd.Id, ct);
        if (despesa is null) return false;

        await _despesaRepository.DeleteAsync(cmd.Id, ct);
        return true;
    }
}
