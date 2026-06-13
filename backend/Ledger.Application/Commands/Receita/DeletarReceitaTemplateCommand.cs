using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Receita;

// ── Command ───────────────────────────────────────────────────────────────────
public record DeletarReceitaTemplateCommand(Guid Id) : IRequest<bool>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class DeletarReceitaTemplateCommandHandler : IRequestHandler<DeletarReceitaTemplateCommand, bool>
{
    private readonly IReceitaTemplateRepository _repo;

    public DeletarReceitaTemplateCommandHandler(IReceitaTemplateRepository repo) => _repo = repo;

    public async Task<bool> Handle(DeletarReceitaTemplateCommand cmd, CancellationToken ct)
    {
        var template = await _repo.GetByIdAsync(cmd.Id, ct);
        if (template is null) return false;
        await _repo.DeleteAsync(cmd.Id, ct);
        return true;
    }
}
