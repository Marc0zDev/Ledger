using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Grupo;

// ── Command ───────────────────────────────────────────────────────────────────
public record DeletarGrupoCommand(Guid GrupoId, Guid UsuarioId) : IRequest<bool>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class DeletarGrupoCommandHandler : IRequestHandler<DeletarGrupoCommand, bool>
{
    private readonly IGrupoRepository _grupoRepository;

    public DeletarGrupoCommandHandler(IGrupoRepository grupoRepository)
        => _grupoRepository = grupoRepository;

    public async Task<bool> Handle(DeletarGrupoCommand cmd, CancellationToken ct)
    {
        var grupo = await _grupoRepository.GetComMembrosAsync(cmd.GrupoId, ct);
        if (grupo is null) return false;

        var membro = grupo.Membros.FirstOrDefault(m => m.UsuarioId == cmd.UsuarioId);
        if (membro?.Role != RoleGrupo.Chefe)
            throw new DomainValidationException(["Apenas o chefe do grupo pode excluí-lo."]);

        await _grupoRepository.DeleteAsync(cmd.GrupoId, ct);
        return true;
    }
}
