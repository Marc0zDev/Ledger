using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using Ledger.Infrastructure.Data;
using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories;

public class MovimentacaoRepository : BaseRepository<MovimentacaoDomain, MovimentacaoModel>, IMovimentacaoRepository
{
    public MovimentacaoRepository(LedgerDbContext context, IMapper mapper) : base(context, mapper) { }

    public async Task<IEnumerable<MovimentacaoDomain>> GetByCofreIdAsync(Guid cofreId, CancellationToken ct = default)
    {
        var models = await DbSet
            .Include(m => m.Usuario)
            .Where(m => m.CofreId == cofreId)
            .OrderByDescending(m => m.Data)
            .ToListAsync(ct);

        return Mapper.Map<IEnumerable<MovimentacaoDomain>>(models);
    }

    public async Task<(IEnumerable<MovimentacaoDomain> Items, int Total)> GetPagedByCofreIdAsync(
        Guid cofreId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = DbSet
            .Include(m => m.Usuario)
            .Where(m => m.CofreId == cofreId)
            .OrderByDescending(m => m.Data);

        var total  = await query.CountAsync(ct);
        var models = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (Mapper.Map<IEnumerable<MovimentacaoDomain>>(models), total);
    }
}
