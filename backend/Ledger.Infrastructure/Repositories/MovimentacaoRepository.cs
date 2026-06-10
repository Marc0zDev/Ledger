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
            .Where(m => m.CofreId == cofreId)
            .OrderByDescending(m => m.Data)
            .ToListAsync(ct);

        return Mapper.Map<IEnumerable<MovimentacaoDomain>>(models);
    }
}
