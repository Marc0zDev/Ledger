using AutoMapper;
using Ledger.Application.DTOs.Grupo;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Grupo;

// ── Command ───────────────────────────────────────────────────────────────────
public record AtualizarGrupoCommand(Guid GrupoId, string Nome, string? Descricao, Guid UsuarioId) : IRequest<GrupoResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class AtualizarGrupoCommandHandler : IRequestHandler<AtualizarGrupoCommand, GrupoResponse>
{
    private readonly IGrupoRepository _grupoRepository;
    private readonly IMapper          _mapper;

    public AtualizarGrupoCommandHandler(IGrupoRepository grupoRepository, IMapper mapper)
    {
        _grupoRepository = grupoRepository;
        _mapper          = mapper;
    }

    public async Task<GrupoResponse> Handle(AtualizarGrupoCommand cmd, CancellationToken ct)
    {
        var grupo = await _grupoRepository.GetComMembrosAsync(cmd.GrupoId, ct)
            ?? throw new DomainValidationException(["Grupo não encontrado."]);

        var membro = grupo.Membros.FirstOrDefault(m => m.UsuarioId == cmd.UsuarioId);
        if (membro?.Role != RoleGrupo.Chefe)
            throw new DomainValidationException(["Apenas o chefe do grupo pode editar as informações."]);

        grupo.Atualizar(cmd.Nome, cmd.Descricao);

        if (!grupo.IsValid)
            throw new DomainValidationException(grupo.Notifications.Select(n => n.Message));

        await _grupoRepository.UpdateAsync(grupo, ct);

        var atualizado = await _grupoRepository.GetComMembrosAsync(cmd.GrupoId, ct);
        return _mapper.Map<GrupoResponse>(atualizado);
    }
}
