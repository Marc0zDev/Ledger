using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using Ledger.Infrastructure.Data;
using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories;

public class ReceitaRepository : BaseRepository<ReceitaDomain, ReceitaModel>, IReceitaRepository
{
    public ReceitaRepository(LedgerDbContext context, IMapper mapper) : base(context, mapper) { }

    public async Task<IEnumerable<ReceitaDomain>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct)
    {
        var models = await DbSet.AsNoTracking()
            .Where(r => r.UsuarioId == usuarioId)
            .OrderByDescending(r => r.DataRecebimento)
            .ToListAsync(ct);
        return Mapper.Map<IEnumerable<ReceitaDomain>>(models);
    }

    public async Task<IEnumerable<ReceitaDomain>> GetByCompetenciaAsync(Guid usuarioId, DateTime competencia, CancellationToken ct)
    {
        var inicio = new DateTime(competencia.Year, competencia.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fim    = inicio.AddMonths(1);

        var models = await DbSet.AsNoTracking()
            .Where(r => r.UsuarioId == usuarioId
                     && r.Competencia >= inicio
                     && r.Competencia < fim)
            .OrderBy(r => r.DataRecebimento)
            .ToListAsync(ct);
        return Mapper.Map<IEnumerable<ReceitaDomain>>(models);
    }

    public async Task<bool> ExisteParaTemplateNoMesAsync(Guid templateId, DateTime competencia, CancellationToken ct)
    {
        var inicio = new DateTime(competencia.Year, competencia.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fim    = inicio.AddMonths(1);

        return await DbSet.AnyAsync(r =>
            r.ReceitaTemplateId == templateId &&
            r.Competencia >= inicio &&
            r.Competencia < fim, ct);
    }
}
