using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Enums;
using Ledger.Domain.Interfaces;
using Ledger.Infrastructure.Data;
using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories;

public class ConviteGrupoRepository : BaseRepository<ConviteGrupoDomain, ConviteGrupoModel>, IConviteGrupoRepository
{
    public ConviteGrupoRepository(LedgerDbContext context, IMapper mapper) : base(context, mapper) { }

    public async Task<ConviteGrupoDomain?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        var model = await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Token == token, ct);
        return model is null ? null : Mapper.Map<ConviteGrupoDomain>(model);
    }

    public async Task<IEnumerable<ConviteGrupoDomain>> GetPendentesByUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var models = await DbSet
            .AsNoTracking()
            .Where(c => c.UsuarioId == usuarioId && c.Status == (int)ConviteStatus.Pendente)
            .ToListAsync(ct);

        return Mapper.Map<IEnumerable<ConviteGrupoDomain>>(models);
    }

    public async Task<bool> ExistePendenteAsync(Guid grupoId, Guid usuarioId, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(
            c => c.GrupoId == grupoId && c.UsuarioId == usuarioId && c.Status == (int)ConviteStatus.Pendente, ct);
    }
}
