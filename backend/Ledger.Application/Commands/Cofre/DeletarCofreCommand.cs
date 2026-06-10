using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Cofre;

// ── Command ───────────────────────────────────────────────────────────────────
public record DeletarCofreCommand(Guid Id) : IRequest<bool>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class DeletarCofreCommandHandler : IRequestHandler<DeletarCofreCommand, bool>
{
    private readonly ICofreRepository _cofreRepository;

    public DeletarCofreCommandHandler(ICofreRepository cofreRepository)
    {
        _cofreRepository = cofreRepository;
    }

    public async Task<bool> Handle(DeletarCofreCommand cmd, CancellationToken ct)
    {
        var cofre = await _cofreRepository.GetByIdAsync(cmd.Id, ct);
        if (cofre is null) return false;

        await _cofreRepository.DeleteAsync(cmd.Id, ct);
        return true;
    }
}
