using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Despesa;

// ── Command ───────────────────────────────────────────────────────────────────
public record DesativarDespesaCommand(Guid Id) : IRequest<bool>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class DesativarDespesaCommandHandler : IRequestHandler<DesativarDespesaCommand, bool>
{
    private readonly IDespesaRepository _despesaRepository;

    public DesativarDespesaCommandHandler(IDespesaRepository despesaRepository)
        => _despesaRepository = despesaRepository;

    public async Task<bool> Handle(DesativarDespesaCommand cmd, CancellationToken ct)
    {
        var despesa = await _despesaRepository.GetByIdAsync(cmd.Id, ct);
        if (despesa is null) return false;
        despesa.Desativar();
        await _despesaRepository.UpdateAsync(despesa, ct);
        return true;
    }
}
