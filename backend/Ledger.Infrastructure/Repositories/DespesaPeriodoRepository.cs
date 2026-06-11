using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using Ledger.Infrastructure.Data;
using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories;

public class DespesaPeriodoRepository : BaseRepository<DespesaPeriodoDomain, DespesaPeriodoModel>, IDespesaPeriodoRepository
{
    public DespesaPeriodoRepository(LedgerDbContext context, IMapper mapper) : base(context, mapper) { }

    public async Task<IEnumerable<DespesaPeriodoDomain>> GetByCompetenciaAsync(
        Guid usuarioId, DateTime competencia, CancellationToken ct = default)
    {
        var inicio = new DateTime(competencia.Year, competencia.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fim    = inicio.AddMonths(1);

        var models = await DbSet
            .Include(d => d.Categoria)
            .Where(d => d.UsuarioId == usuarioId && d.Competencia >= inicio && d.Competencia < fim)
            .OrderBy(d => d.PagaEm.HasValue) // não pagos primeiro
            .ThenBy(d => d.Descricao)
            .ToListAsync(ct);

        return Mapper.Map<IEnumerable<DespesaPeriodoDomain>>(models);
    }

    public async Task<IEnumerable<DespesaPeriodoDomain>> GetPendentesAsync(
        Guid usuarioId, CancellationToken ct = default)
    {
        var models = await DbSet
            .Include(d => d.Categoria)
            .Where(d => d.UsuarioId == usuarioId && d.PagaEm == null)
            .OrderBy(d => d.Competencia)
            .ToListAsync(ct);

        return Mapper.Map<IEnumerable<DespesaPeriodoDomain>>(models);
    }

    public async Task<bool> ExisteParaTemplateNoMesAsync(
        Guid despesaId, DateTime competencia, CancellationToken ct = default)
    {
        var inicio = new DateTime(competencia.Year, competencia.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fim    = inicio.AddMonths(1);
        return await DbSet.AnyAsync(
            d => d.DespesaId == despesaId && d.Competencia >= inicio && d.Competencia < fim, ct);
    }
}
