using AutoMapper;
using Ledger.Application.DTOs.Participante;
using Ledger.Application.Events;
using Ledger.Domain.Entities;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Participante;

// ── Command ───────────────────────────────────────────────────────────────────
public record AdicionarParticipanteCommand(Guid CofreId, Guid UsuarioId) : IRequest<ParticipanteResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class AdicionarParticipanteCommandHandler : IRequestHandler<AdicionarParticipanteCommand, ParticipanteResponse>
{
    private readonly ICofreRepository        _cofreRepository;
    private readonly IParticipanteRepository _participanteRepository;
    private readonly IUsuarioRepository      _usuarioRepository;
    private readonly IDomainEventDispatcher  _dispatcher;
    private readonly IMapper                 _mapper;

    public AdicionarParticipanteCommandHandler(
        ICofreRepository cofreRepository,
        IParticipanteRepository participanteRepository,
        IUsuarioRepository usuarioRepository,
        IDomainEventDispatcher dispatcher,
        IMapper mapper)
    {
        _cofreRepository        = cofreRepository;
        _participanteRepository = participanteRepository;
        _usuarioRepository      = usuarioRepository;
        _dispatcher             = dispatcher;
        _mapper                 = mapper;
    }

    public async Task<ParticipanteResponse> Handle(AdicionarParticipanteCommand cmd, CancellationToken ct)
    {
        var cofre = await _cofreRepository.GetComDetalhesAsync(cmd.CofreId, ct)
            ?? throw new DomainValidationException(["Cofre não encontrado."]);

        if (cofre.CriadoPorUsuarioId == cmd.UsuarioId)
            throw new DomainValidationException(["O criador do cofre já é participante automaticamente."]);

        await (_usuarioRepository.GetByIdAsync(cmd.UsuarioId, ct)
            ?? throw new DomainValidationException(["Usuário não encontrado."]));

        var membership = ParticipanteDomain.Criar(cmd.CofreId, cmd.UsuarioId);
        cofre.AdicionarParticipante(membership);

        if (!cofre.IsValid)
            throw new DomainValidationException(cofre.Notifications.Select(n => n.Message));

        await _participanteRepository.AddAsync(membership, ct);

        // Despacha ParticipanteAdicionadoEvent → envia e-mail de convite
        await _dispatcher.DispatchAsync(cofre, ct);

        return _mapper.Map<ParticipanteResponse>(membership);
    }
}
