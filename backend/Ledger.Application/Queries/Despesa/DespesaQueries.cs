using AutoMapper;
using Ledger.Application.DTOs.Despesa;
using Ledger.Domain.Interfaces;
using MediatR;

namespace Ledger.Application.Queries.Despesa;

// ── Listar por Usuário ────────────────────────────────────────────────────────
public record ListarDespesasQuery(Guid UsuarioId) : IRequest<IEnumerable<DespesaResponse>>;

public class ListarDespesasQueryHandler : IRequestHandler<ListarDespesasQuery, IEnumerable<DespesaResponse>>
{
    private readonly IDespesaRepository _despesaRepository;
    private readonly IMapper            _mapper;

    public ListarDespesasQueryHandler(IDespesaRepository despesaRepository, IMapper mapper)
    {
        _despesaRepository = despesaRepository;
        _mapper            = mapper;
    }

    public async Task<IEnumerable<DespesaResponse>> Handle(ListarDespesasQuery query, CancellationToken ct)
    {
        var despesas = await _despesaRepository.GetByUsuarioIdAsync(query.UsuarioId, ct);
        return _mapper.Map<IEnumerable<DespesaResponse>>(despesas);
    }
}

// ── Pendentes (próximas a vencer) ─────────────────────────────────────────────
public record ListarDespesasPendentesQuery(Guid UsuarioId, DateTime? VencimentoAte = null)
    : IRequest<IEnumerable<DespesaResponse>>;

public class ListarDespesasPendentesQueryHandler
    : IRequestHandler<ListarDespesasPendentesQuery, IEnumerable<DespesaResponse>>
{
    private readonly IDespesaRepository _despesaRepository;
    private readonly IMapper            _mapper;

    public ListarDespesasPendentesQueryHandler(IDespesaRepository despesaRepository, IMapper mapper)
    {
        _despesaRepository = despesaRepository;
        _mapper            = mapper;
    }

    public async Task<IEnumerable<DespesaResponse>> Handle(ListarDespesasPendentesQuery query, CancellationToken ct)
    {
        var despesas = await _despesaRepository.GetPendentesAsync(query.UsuarioId, query.VencimentoAte, ct);
        return _mapper.Map<IEnumerable<DespesaResponse>>(despesas);
    }
}

// ── Obter por Id ─────────────────────────────────────────────────────────────
public record ObterDespesaQuery(Guid Id) : IRequest<DespesaResponse?>;

public class ObterDespesaQueryHandler : IRequestHandler<ObterDespesaQuery, DespesaResponse?>
{
    private readonly IDespesaRepository _despesaRepository;
    private readonly IMapper            _mapper;

    public ObterDespesaQueryHandler(IDespesaRepository despesaRepository, IMapper mapper)
    {
        _despesaRepository = despesaRepository;
        _mapper            = mapper;
    }

    public async Task<DespesaResponse?> Handle(ObterDespesaQuery query, CancellationToken ct)
    {
        var despesa = await _despesaRepository.GetByIdAsync(query.Id, ct);
        return despesa is null ? null : _mapper.Map<DespesaResponse>(despesa);
    }
}
