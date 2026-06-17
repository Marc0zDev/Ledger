using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Grupo;

// ── Command ───────────────────────────────────────────────────────────────────
public record RemoverMembroGrupoCommand(Guid GrupoId, Guid MembroId, Guid SolicitanteId) : IRequest<bool>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class RemoverMembroGrupoCommandHandler : IRequestHandler<RemoverMembroGrupoCommand, bool>
{
    private readonly IGrupoRepository _grupoRepository;

    public RemoverMembroGrupoCommandHandler(IGrupoRepository grupoRepository)
        => _grupoRepository = grupoRepository;

    public async Task<bool> Handle(RemoverMembroGrupoCommand cmd, CancellationToken ct)
    {
        var grupo = await _grupoRepository.GetComMembrosAsync(cmd.GrupoId, ct)
            ?? throw new DomainValidationException(["Grupo não encontrado."]);

        var solicitante = grupo.Membros.FirstOrDefault(m => m.UsuarioId == cmd.SolicitanteId);
        if (solicitante?.Role != RoleGrupo.Chefe)
            throw new DomainValidationException(["Apenas o chefe do grupo pode remover membros."]);

        var membro = grupo.Membros.FirstOrDefault(m => m.Id == cmd.MembroId);
        if (membro is null) return false;

        if (membro.Role == RoleGrupo.Chefe)
            throw new DomainValidationException(["Não é possível remover o chefe do grupo."]);

        await _grupoRepository.DeleteMembroAsync(cmd.MembroId, ct);
        return true;
    }
}
