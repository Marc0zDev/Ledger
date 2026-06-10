using Ledger.Application.DTOs.Cofre;
using Ledger.Application.Events;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;
using MediatR;
using AutoMapper;

namespace Ledger.Application.Commands.Cofre;

public class ConcluirCofreCommandHandler : IRequestHandler<ConcluirCofreCommand, CofreResponse?>
{
    private readonly ICofreRepository        _cofreRepository;
    private readonly IDomainEventDispatcher  _dispatcher;
    private readonly IMapper                 _mapper;

    public ConcluirCofreCommandHandler(
        ICofreRepository cofreRepository,
        IDomainEventDispatcher dispatcher,
        IMapper mapper)
    {
        _cofreRepository = cofreRepository;
        _dispatcher      = dispatcher;
        _mapper          = mapper;
    }

    public async Task<CofreResponse?> Handle(ConcluirCofreCommand request, CancellationToken ct)
    {
        var cofre = await _cofreRepository.GetComDetalhesAsync(request.CofreId, ct);
        if (cofre is null) return null;

        cofre.Concluir();

        if (!cofre.IsValid)
            throw new DomainValidationException(cofre.Notifications.Select(n => n.Message));

        await _cofreRepository.UpdateAsync(cofre, ct);

        // Despacha CofreConcluídoEvent → CofreConcluídoEventHandler envia os e-mails
        await _dispatcher.DispatchAsync(cofre, ct);

        return _mapper.Map<CofreResponse>(cofre);
    }
}
