using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using Ledger.Infrastructure.Data;
using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories;

public class GrupoRepository : BaseRepository<GrupoDomain, GrupoModel>, IGrupoRepository
{
    public GrupoRepository(LedgerDbContext context, IMapper mapper) : base(context, mapper) { }

    public async Task<IEnumerable<GrupoDomain>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var models = await DbSet
            .Include(g => g.Membros).ThenInclude(m => m.Usuario)
            .Where(g => g.Membros.Any(m => m.UsuarioId == usuarioId))
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(ct);

        return Mapper.Map<IEnumerable<GrupoDomain>>(models);
    }

    public async Task<GrupoDomain?> GetComMembrosAsync(Guid grupoId, CancellationToken ct = default)
    {
        var model = await DbSet
            .Include(g => g.Membros).ThenInclude(m => m.Usuario)
            .FirstOrDefaultAsync(g => g.Id == grupoId, ct);

        return model is null ? null : Mapper.Map<GrupoDomain>(model);
    }

    public async Task AddMembroAsync(GrupoMembroDomain membro, CancellationToken ct = default)
    {
        var model = Mapper.Map<GrupoMembroModel>(membro);
        await Context.Set<GrupoMembroModel>().AddAsync(model, ct);
        await Context.SaveChangesAsync(ct);
    }

    public async Task DeleteMembroAsync(Guid membroId, CancellationToken ct = default)
    {
        var model = await Context.Set<GrupoMembroModel>().FindAsync(new object[] { membroId }, ct);
        if (model is not null)
        {
            Context.Set<GrupoMembroModel>().Remove(model);
            await Context.SaveChangesAsync(ct);
        }
    }
}
