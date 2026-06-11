using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using Ledger.Infrastructure.Data;
using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories;

public class CofreRepository : BaseRepository<CofreDomain, CofreModel>, ICofreRepository
{
    public CofreRepository(LedgerDbContext context, IMapper mapper) : base(context, mapper) { }

    public async Task<CofreDomain?> GetComDetalhesAsync(Guid id, CancellationToken ct = default)
    {
        var model = await DbSet
            .Include(c => c.Participantes)
                .ThenInclude(p => p.Usuario)
            .Include(c => c.Movimentacoes)
                .ThenInclude(m => m.Usuario)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        return model is null ? null : Mapper.Map<CofreDomain>(model);
    }

    public async Task<IEnumerable<CofreDomain>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var models = await DbSet
            .AsNoTracking()
            .Include(c => c.Participantes)
                .ThenInclude(p => p.Usuario)
            .Include(c => c.Movimentacoes)
            .Where(c => c.CriadoPorUsuarioId == usuarioId
                     || c.Participantes.Any(p => p.UsuarioId == usuarioId))
            .ToListAsync(ct);

        return Mapper.Map<IEnumerable<CofreDomain>>(models);
    }
}
