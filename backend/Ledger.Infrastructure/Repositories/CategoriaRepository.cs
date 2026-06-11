using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using Ledger.Infrastructure.Data;
using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories;

public class CategoriaRepository : BaseRepository<CategoriaDomain, CategoriaModel>, ICategoriaRepository
{
    public CategoriaRepository(LedgerDbContext context, IMapper mapper) : base(context, mapper) { }

    public async Task<IEnumerable<CategoriaDomain>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var models = await DbSet
            .Where(c => c.UsuarioId == null || c.UsuarioId == usuarioId)
            .OrderBy(c => c.UsuarioId == null ? 0 : 1) // sistema primeiro
            .ThenBy(c => c.Nome)
            .ToListAsync(ct);

        return Mapper.Map<IEnumerable<CategoriaDomain>>(models);
    }

    public override async Task<CategoriaDomain?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var model = await DbSet.FindAsync(new object[] { id }, ct);
        return model is null ? null : Mapper.Map<CategoriaDomain>(model);
    }
}
