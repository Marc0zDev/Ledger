using Ledger.Application.DTOs.Despesa;

namespace Ledger.Application.Interfaces;

public interface IDespesaService
{
    Task<DespesaResponse?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> RemoverAsync(Guid id, CancellationToken ct = default);
}
