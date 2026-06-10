using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using Ledger.Infrastructure.Data;
using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories;

public class ParticipanteRepository : BaseRepository<ParticipanteDomain, ParticipanteModel>, IParticipanteRepository
{
    public ParticipanteRepository(LedgerDbContext context, IMapper mapper) : base(context, mapper) { }

    public async Task<IEnumerable<ParticipanteDomain>> GetByCofreIdAsync(Guid cofreId, CancellationToken ct = default)
    {
        var models = await DbSet
            .Include(p => p.Usuario)
            .Where(p => p.CofreId == cofreId)
            .ToListAsync(ct);

        return Mapper.Map<IEnumerable<ParticipanteDomain>>(models);
    }

    public async Task<ParticipanteDomain?> GetByCofreIdAndUsuarioIdAsync(Guid cofreId, Guid usuarioId, CancellationToken ct = default)
    {
        var model = await DbSet
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.CofreId == cofreId && p.UsuarioId == usuarioId, ct);

        return model is null ? null : Mapper.Map<ParticipanteDomain>(model);
    }
}
