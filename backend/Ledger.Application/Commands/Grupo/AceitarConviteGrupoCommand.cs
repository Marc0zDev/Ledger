using AutoMapper;
using Ledger.Application.DTOs.Grupo;
using Ledger.Domain.Entities;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Grupo;

// ── Command ───────────────────────────────────────────────────────────────────
public record AceitarConviteGrupoCommand(string Token, Guid UsuarioId) : IRequest<GrupoMembroResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class AceitarConviteGrupoCommandHandler : IRequestHandler<AceitarConviteGrupoCommand, GrupoMembroResponse>
{
    private readonly IConviteGrupoRepository _conviteGrupoRepository;
    private readonly IGrupoRepository        _grupoRepository;
    private readonly IMapper                 _mapper;

    public AceitarConviteGrupoCommandHandler(
        IConviteGrupoRepository conviteGrupoRepository,
        IGrupoRepository grupoRepository,
        IMapper mapper)
    {
        _conviteGrupoRepository = conviteGrupoRepository;
        _grupoRepository        = grupoRepository;
        _mapper                 = mapper;
    }

    public async Task<GrupoMembroResponse> Handle(AceitarConviteGrupoCommand cmd, CancellationToken ct)
    {
        var convite = await _conviteGrupoRepository.GetByTokenAsync(cmd.Token, ct)
            ?? throw new DomainValidationException(["Convite não encontrado."]);

        if (convite.UsuarioId != cmd.UsuarioId)
            throw new DomainValidationException(["Este convite não pertence ao usuário autenticado."]);

        convite.Aceitar();

        if (!convite.IsValid)
            throw new DomainValidationException(convite.Notifications.Select(n => n.Message));

        var membro = GrupoMembroDomain.Criar(convite.GrupoId, convite.UsuarioId);

        if (!membro.IsValid)
            throw new DomainValidationException(membro.Notifications.Select(n => n.Message));

        await _grupoRepository.AddMembroAsync(membro, ct);
        await _conviteGrupoRepository.UpdateAsync(convite, ct);

        return _mapper.Map<GrupoMembroResponse>(membro);
    }
}
