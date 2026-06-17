using AutoMapper;
using Ledger.Application.DTOs.Grupo;
using Ledger.Domain.Entities;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Grupo;

// ── Command ───────────────────────────────────────────────────────────────────
public record AdicionarMembroGrupoCommand(Guid GrupoId, Guid UsuarioId, Guid SolicitanteId) : IRequest<GrupoMembroResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class AdicionarMembroGrupoCommandHandler : IRequestHandler<AdicionarMembroGrupoCommand, GrupoMembroResponse>
{
    private readonly IGrupoRepository  _grupoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper           _mapper;

    public AdicionarMembroGrupoCommandHandler(IGrupoRepository grupoRepository, IUsuarioRepository usuarioRepository, IMapper mapper)
    {
        _grupoRepository   = grupoRepository;
        _usuarioRepository = usuarioRepository;
        _mapper            = mapper;
    }

    public async Task<GrupoMembroResponse> Handle(AdicionarMembroGrupoCommand cmd, CancellationToken ct)
    {
        var grupo = await _grupoRepository.GetComMembrosAsync(cmd.GrupoId, ct)
            ?? throw new DomainValidationException(["Grupo não encontrado."]);

        var solicitante = grupo.Membros.FirstOrDefault(m => m.UsuarioId == cmd.SolicitanteId);
        if (solicitante?.Role != RoleGrupo.Chefe)
            throw new DomainValidationException(["Apenas o chefe do grupo pode adicionar membros."]);

        var usuario = await _usuarioRepository.GetByIdAsync(cmd.UsuarioId, ct)
            ?? throw new DomainValidationException(["Usuário não encontrado."]);

        var novoMembro = GrupoMembroDomain.Criar(cmd.GrupoId, cmd.UsuarioId, RoleGrupo.Membro);
        grupo.AdicionarMembro(novoMembro);

        if (!grupo.IsValid)
            throw new DomainValidationException(grupo.Notifications.Select(n => n.Message));

        await _grupoRepository.AddMembroAsync(novoMembro, ct);

        var response = _mapper.Map<GrupoMembroResponse>(novoMembro);
        response.Nome  = usuario.Nome;
        response.Email = usuario.Email;
        return response;
    }
}
