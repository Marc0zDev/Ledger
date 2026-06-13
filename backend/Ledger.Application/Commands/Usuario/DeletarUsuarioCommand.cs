using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Usuario;

// ── Command ───────────────────────────────────────────────────────────────────
public record DeletarUsuarioCommand(Guid Id) : IRequest<bool>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class DeletarUsuarioCommandHandler : IRequestHandler<DeletarUsuarioCommand, bool>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public DeletarUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
        => _usuarioRepository = usuarioRepository;

    public async Task<bool> Handle(DeletarUsuarioCommand cmd, CancellationToken ct)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(cmd.Id, ct);
        if (usuario is null) return false;

        await _usuarioRepository.DeleteAsync(cmd.Id, ct);
        return true;
    }
}
