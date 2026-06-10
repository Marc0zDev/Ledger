using AutoMapper;
using Ledger.Application.DTOs.Movimentacao;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Movimentacao;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ListarMovimentacoesQuery(Guid CofreId) : IRequest<IEnumerable<MovimentacaoResponse>>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ListarMovimentacoesQueryHandler : IRequestHandler<ListarMovimentacoesQuery, IEnumerable<MovimentacaoResponse>>
{
    private readonly IMovimentacaoRepository _movimentacaoRepository;
    private readonly IMapper                 _mapper;

    public ListarMovimentacoesQueryHandler(IMovimentacaoRepository movimentacaoRepository, IMapper mapper)
    {
        _movimentacaoRepository = movimentacaoRepository;
        _mapper                 = mapper;
    }

    public async Task<IEnumerable<MovimentacaoResponse>> Handle(ListarMovimentacoesQuery query, CancellationToken ct)
    {
        var movs = await _movimentacaoRepository.GetByCofreIdAsync(query.CofreId, ct);
        return _mapper.Map<IEnumerable<MovimentacaoResponse>>(movs);
    }
}
