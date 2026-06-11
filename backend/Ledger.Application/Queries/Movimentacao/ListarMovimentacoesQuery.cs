using AutoMapper;
using Ledger.Application.DTOs;
using Ledger.Application.DTOs.Movimentacao;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Movimentacao;

// ── Query ─────────────────────────────────────────────────────────────────────
public record ListarMovimentacoesQuery(Guid CofreId, int Page = 1, int PageSize = 5)
    : IRequest<PagedResult<MovimentacaoResponse>>;

// ── Handler ───────────────────────────────────────────────────────────────────
public class ListarMovimentacoesQueryHandler
    : IRequestHandler<ListarMovimentacoesQuery, PagedResult<MovimentacaoResponse>>
{
    private readonly IMovimentacaoRepository _movimentacaoRepository;
    private readonly IMapper                 _mapper;

    public ListarMovimentacoesQueryHandler(IMovimentacaoRepository movimentacaoRepository, IMapper mapper)
    {
        _movimentacaoRepository = movimentacaoRepository;
        _mapper                 = mapper;
    }

    public async Task<PagedResult<MovimentacaoResponse>> Handle(ListarMovimentacoesQuery query, CancellationToken ct)
    {
        var page     = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);

        var (items, total) = await _movimentacaoRepository.GetPagedByCofreIdAsync(query.CofreId, page, pageSize, ct);

        return new PagedResult<MovimentacaoResponse>
        {
            Items    = _mapper.Map<IEnumerable<MovimentacaoResponse>>(items),
            Page     = page,
            PageSize = pageSize,
            Total    = total,
        };
    }
}
