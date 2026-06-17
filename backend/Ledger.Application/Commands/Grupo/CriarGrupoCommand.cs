using AutoMapper;
using Ledger.Application.DTOs.Grupo;
using Ledger.Domain.Entities;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Grupo;

// ── Command ───────────────────────────────────────────────────────────────────
public record CriarGrupoCommand(string Nome, string? Descricao, Guid UsuarioId) : IRequest<GrupoResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class CriarGrupoCommandHandler : IRequestHandler<CriarGrupoCommand, GrupoResponse>
{
    private readonly IGrupoRepository _grupoRepository;
    private readonly IMapper          _mapper;

    public CriarGrupoCommandHandler(IGrupoRepository grupoRepository, IMapper mapper)
    {
        _grupoRepository = grupoRepository;
        _mapper          = mapper;
    }

    public async Task<GrupoResponse> Handle(CriarGrupoCommand cmd, CancellationToken ct)
    {
        var grupo = GrupoDomain.Criar(cmd.Nome, cmd.Descricao, cmd.UsuarioId);

        if (!grupo.IsValid)
            throw new DomainValidationException(grupo.Notifications.Select(n => n.Message));

        await _grupoRepository.AddAsync(grupo, ct);

        // Adiciona o criador como Chefe
        var chefe = GrupoMembroDomain.Criar(grupo.Id, cmd.UsuarioId, RoleGrupo.Chefe);
        await _grupoRepository.AddMembroAsync(chefe, ct);

        var criado = await _grupoRepository.GetComMembrosAsync(grupo.Id, ct);
        return _mapper.Map<GrupoResponse>(criado);
    }
}
