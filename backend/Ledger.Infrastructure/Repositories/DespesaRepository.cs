using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using Ledger.Infrastructure.Data;
using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories;

public class DespesaRepository : BaseRepository<DespesaDomain, DespesaModel>, IDespesaRepository
{
    public DespesaRepository(LedgerDbContext context, IMapper mapper) : base(context, mapper) { }

    public async Task<IEnumerable<DespesaDomain>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var models = await DbSet
            .Where(d => d.UsuarioId == usuarioId)
            .OrderByDescending(d => d.DataVencimento)
            .ToListAsync(ct);

        return Mapper.Map<IEnumerable<DespesaDomain>>(models);
    }

    public async Task<IEnumerable<DespesaDomain>> GetPendentesAsync(
        Guid usuarioId, DateTime? vencimentoAte = null, CancellationToken ct = default)
    {
        var query = DbSet
            .Where(d => d.UsuarioId == usuarioId && !d.Paga);

        if (vencimentoAte.HasValue)
            query = query.Where(d => d.DataVencimento <= vencimentoAte.Value);

        var models = await query
            .OrderBy(d => d.DataVencimento)
            .ToListAsync(ct);

        return Mapper.Map<IEnumerable<DespesaDomain>>(models);
    }
}
