using AutoMapper;
using Ledger.Application.DTOs.Cofre;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Cofre;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ObterCofreQuery(Guid Id) : IRequest<CofreResponse?>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ObterCofreQueryHandler : IRequestHandler<ObterCofreQuery, CofreResponse?>
{
    private readonly ICofreRepository _cofreRepository;
    private readonly IMapper          _mapper;

    public ObterCofreQueryHandler(ICofreRepository cofreRepository, IMapper mapper)
    {
        _cofreRepository = cofreRepository;
        _mapper          = mapper;
    }

    public async Task<CofreResponse?> Handle(ObterCofreQuery query, CancellationToken ct)
    {
        var cofre = await _cofreRepository.GetByIdAsync(query.Id, ct);
        return cofre is null ? null : _mapper.Map<CofreResponse>(cofre);
    }
}
