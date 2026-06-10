using AutoMapper;
using Ledger.Application.DTOs.Cofre;
using Ledger.Application.Events;
using Ledger.Application.Interfaces;
using Ledger.Domain.Entities;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;

namespace Ledger.Application.Services;

public class CofreService : ICofreService
{
    private readonly ICofreRepository        _cofreRepository;
    private readonly IParticipanteRepository _participanteRepository;
    private readonly IDomainEventDispatcher  _dispatcher;
    private readonly IMapper                 _mapper;

    public CofreService(
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

    public async Task<IEnumerable<CofreResponse>> ListarAsync(CancellationToken ct = default)
    {
        var cofres = await _cofreRepository.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<CofreResponse>>(cofres);
    }

    public async Task<CofreResponse?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var cofre = await _cofreRepository.GetByIdAsync(id, ct);
        return cofre is null ? null : _mapper.Map<CofreResponse>(cofre);
    }

    public async Task<CofreResponse?> ObterComDetalhesAsync(Guid id, CancellationToken ct = default)
    {
        var cofre = await _cofreRepository.GetComDetalhesAsync(id, ct);
        return cofre is null ? null : _mapper.Map<CofreResponse>(cofre);
    }

    public async Task<CofreResponse> CriarAsync(CriarCofreRequest request, Guid criadoPorUsuarioId, CancellationToken ct = default)
    {
        var cofre = CofreDomain.Criar(request.Nome, request.Meta, criadoPorUsuarioId, request.Descricao);

        if (!cofre.IsValid)
            throw new DomainValidationException(cofre.Notifications.Select(n => n.Message));

        await _cofreRepository.AddAsync(cofre, ct);

        // Criador entra automaticamente como primeiro participante
        var participante = ParticipanteDomain.Criar(cofre.Id, criadoPorUsuarioId);
        await _participanteRepository.AddAsync(participante, ct);

        return _mapper.Map<CofreResponse>(cofre);
    }

    public async Task<CofreResponse?> AtualizarAsync(Guid id, AtualizarCofreRequest request, CancellationToken ct = default)
    {
        var cofre = await _cofreRepository.GetByIdAsync(id, ct);
        if (cofre is null) return null;

        cofre.Atualizar(request.Nome, request.Meta, request.Descricao,
            Enum.TryParse<Ledger.Domain.Enums.CategoriaCofre>(request.Categoria, true, out var cat) ? cat : Ledger.Domain.Enums.CategoriaCofre.Outro);

        if (!cofre.IsValid)
            throw new DomainValidationException(cofre.Notifications.Select(n => n.Message));

        await _cofreRepository.UpdateAsync(cofre, ct);
        return _mapper.Map<CofreResponse>(cofre);
    }

    public async Task<bool> DeletarAsync(Guid id, CancellationToken ct = default)
    {
        var cofre = await _cofreRepository.GetByIdAsync(id, ct);
        if (cofre is null) return false;

        await _cofreRepository.DeleteAsync(id, ct);
        return true;
    }

    public async Task<CofreResponse?> ConcluirAsync(Guid id, CancellationToken ct = default)
    {
        var cofre = await _cofreRepository.GetComDetalhesAsync(id, ct);
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
