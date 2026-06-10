using AutoMapper;
using Ledger.Application.DTOs.Participante;
using Ledger.Domain.Entities;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Participante;

// ── Command ───────────────────────────────────────────────────────────────────
public record AceitarConviteCommand(string Token, Guid UsuarioId) : IRequest<ParticipanteResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class AceitarConviteCommandHandler : IRequestHandler<AceitarConviteCommand, ParticipanteResponse>
{
    private readonly IConviteRepository      _conviteRepository;
    private readonly IParticipanteRepository _participanteRepository;
    private readonly IMapper                 _mapper;

    public AceitarConviteCommandHandler(
        IConviteRepository conviteRepository,
        IParticipanteRepository participanteRepository,
        IMapper mapper)
    {
        _conviteRepository      = conviteRepository;
        _participanteRepository = participanteRepository;
        _mapper                 = mapper;
    }

    public async Task<ParticipanteResponse> Handle(AceitarConviteCommand cmd, CancellationToken ct)
    {
        var convite = await _conviteRepository.GetByTokenAsync(cmd.Token, ct)
            ?? throw new DomainValidationException(["Convite não encontrado."]);

        if (convite.UsuarioId != cmd.UsuarioId)
            throw new DomainValidationException(["Este convite não pertence ao usuário autenticado."]);

        convite.Aceitar();

        if (!convite.IsValid)
            throw new DomainValidationException(convite.Notifications.Select(n => n.Message));

        // Cria o vínculo diretamente — sem passar pelo agregado CofreDomain para evitar
        // revalidação da lista de participantes já carregada em memória.
        var membership = ParticipanteDomain.Criar(convite.CofreId, convite.UsuarioId);

        if (!membership.IsValid)
            throw new DomainValidationException(membership.Notifications.Select(n => n.Message));

        await _participanteRepository.AddAsync(membership, ct);
        await _conviteRepository.UpdateAsync(convite, ct);

        return _mapper.Map<ParticipanteResponse>(membership);
    }
}
