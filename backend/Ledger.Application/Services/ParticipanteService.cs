using AutoMapper;
using Ledger.Application.DTOs.Participante;
using Ledger.Application.Events;
using Ledger.Application.Interfaces;
using Ledger.Domain.Entities;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;

namespace Ledger.Application.Services;

public class ParticipanteService : IParticipanteService
{
    private readonly IParticipanteRepository _participanteRepository;
    private readonly ICofreRepository        _cofreRepository;
    private readonly IUsuarioRepository      _usuarioRepository;
    private readonly IDomainEventDispatcher  _dispatcher;
    private readonly IMapper                 _mapper;

    public ParticipanteService(
        IParticipanteRepository participanteRepository,
        ICofreRepository cofreRepository,
        IUsuarioRepository usuarioRepository,
        IDomainEventDispatcher dispatcher,
        IMapper mapper)
    {
        _participanteRepository = participanteRepository;
        _cofreRepository        = cofreRepository;
        _usuarioRepository      = usuarioRepository;
        _dispatcher             = dispatcher;
        _mapper                 = mapper;
    }

    public async Task<IEnumerable<ParticipanteResponse>> ListarPorCofreAsync(Guid cofreId, CancellationToken ct = default)
    {
        var participantes = await _participanteRepository.GetByCofreIdAsync(cofreId, ct);
        return _mapper.Map<IEnumerable<ParticipanteResponse>>(participantes);
    }

    public async Task<ParticipanteResponse> AdicionarAsync(Guid cofreId, CriarParticipanteRequest request, CancellationToken ct = default)
    {
        var cofre = await _cofreRepository.GetComDetalhesAsync(cofreId, ct)
            ?? throw new DomainValidationException(["Cofre não encontrado."]);

        if (cofre.CriadoPorUsuarioId == request.UsuarioId)
            throw new DomainValidationException(["O criador do cofre já é participante automaticamente."]);

        var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId, ct)
            ?? throw new DomainValidationException(["Usuário não encontrado."]);

        var membership = ParticipanteDomain.Criar(cofreId, usuario.Id);
        cofre.AdicionarParticipante(membership);

        if (!cofre.IsValid)
            throw new DomainValidationException(cofre.Notifications.Select(n => n.Message));

        await _participanteRepository.AddAsync(membership, ct);

        // Despacha ParticipanteAdicionadoEvent → ParticipanteAdicionadoEventHandler envia convite
        await _dispatcher.DispatchAsync(cofre, ct);

        return _mapper.Map<ParticipanteResponse>(membership);
    }

    public async Task<bool> RemoverAsync(Guid cofreId, Guid participanteId, CancellationToken ct = default)
    {
        var participante = await _participanteRepository.GetByCofreIdAndUsuarioIdAsync(cofreId, participanteId, ct);
        if (participante is null) return false;

        await _participanteRepository.DeleteAsync(participante.Id, ct);
        return true;
    }
}
