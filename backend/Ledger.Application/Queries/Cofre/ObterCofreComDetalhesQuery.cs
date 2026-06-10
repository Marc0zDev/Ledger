using AutoMapper;
using Ledger.Application.DTOs.Cofre;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Cofre;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ObterCofreComDetalhesQuery(Guid Id) : IRequest<CofreResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ObterCofreComDetalhesQueryHandler : IRequestHandler<ObterCofreComDetalhesQuery, CofreResponse?>
{
    private readonly ICofreRepository _cofreRepository;
    private readonly IMapper          _mapper;

    public ObterCofreComDetalhesQueryHandler(ICofreRepository cofreRepository, IMapper mapper)
    {
        _cofreRepository = cofreRepository;
        _mapper          = mapper;
    }

    public async Task<CofreResponse?> Handle(ObterCofreComDetalhesQuery query, CancellationToken ct)
    {
        var cofre = await _cofreRepository.GetComDetalhesAsync(query.Id, ct);
        return cofre is null ? null : _mapper.Map<CofreResponse>(cofre);
    }
}
