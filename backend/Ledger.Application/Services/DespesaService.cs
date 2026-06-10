using AutoMapper;
using Ledger.Application.DTOs.Despesa;
using Ledger.Application.Interfaces;
using Ledger.Domain.Entities;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Interfaces;

namespace Ledger.Application.Services;

public class DespesaService : IDespesaService
{
    private readonly IDespesaRepository _despesaRepository;
    private readonly IMapper            _mapper;

    public DespesaService(IDespesaRepository despesaRepository, IMapper mapper)
    {
        _despesaRepository = despesaRepository;
        _mapper            = mapper;
    }

    public async Task<DespesaResponse?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var despesa = await _despesaRepository.GetByIdAsync(id, ct);
        return despesa is null ? null : _mapper.Map<DespesaResponse>(despesa);
    }

    public async Task<bool> RemoverAsync(Guid id, CancellationToken ct = default)
    {
        var despesa = await _despesaRepository.GetByIdAsync(id, ct);
        if (despesa is null) return false;

        await _despesaRepository.DeleteAsync(id, ct);
        return true;
    }
}
