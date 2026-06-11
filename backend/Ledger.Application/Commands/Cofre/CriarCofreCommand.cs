using AutoMapper;
using Ledger.Application.DTOs.Cofre;
using Ledger.Application.Events;
using Ledger.Domain.Entities;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Commands.Cofre;

// ── Command ──────────────────────────────────────────────────────────────────
public record CriarCofreCommand(
    string            Nome,
    decimal           Meta,
    Guid              CriadoPorUsuarioId,
    string?           Descricao   = null,
    CategoriaCofre    Categoria   = CategoriaCofre.Outro,
    VisibilidadeCofre Visibilidade = VisibilidadeCofre.Privado) : IRequest<CofreResponse>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class CriarCofreCommandHandler : IRequestHandler<CriarCofreCommand, CofreResponse>
{
    private readonly ICofreRepository        _cofreRepository;
    private readonly IParticipanteRepository _participanteRepository;
    private readonly IDomainEventDispatcher  _dispatcher;
    private readonly IMapper                 _mapper;

    public CriarCofreCommandHandler(
        ICofreRepository cofreRepository,
        IParticipanteRepository participanteRepository,
        IDomainEventDispatcher dispatcher,
        IMapper mapper)
    {
        _cofreRepository        = cofreRepository;
        _participanteRepository = participanteRepository;
        _dispatcher             = dispatcher;
        _mapper                 = mapper;
    }

    public async Task<CofreResponse> Handle(CriarCofreCommand cmd, CancellationToken ct)
    {
        var cofre = CofreDomain.Criar(cmd.Nome, cmd.Meta, cmd.CriadoPorUsuarioId, cmd.Descricao, cmd.Categoria, cmd.Visibilidade);

        if (!cofre.IsValid)
            throw new DomainValidationException(cofre.Notifications.Select(n => n.Message));

        await _cofreRepository.AddAsync(cofre, ct);

        var participante = ParticipanteDomain.Criar(cofre.Id, cmd.CriadoPorUsuarioId, RoleParticipante.Admin);
        await _participanteRepository.AddAsync(participante, ct);

        await _dispatcher.DispatchAsync(cofre, ct);

        return _mapper.Map<CofreResponse>(cofre);
    }
}

