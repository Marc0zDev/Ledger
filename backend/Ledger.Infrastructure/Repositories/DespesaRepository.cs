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
            .Include(d => d.Categoria)
            .Where(d => d.UsuarioId == usuarioId)
            .OrderBy(d => d.Nome)
            .ToListAsync(ct);

        return Mapper.Map<IEnumerable<DespesaDomain>>(models);
    }

    public async Task<(IEnumerable<DespesaDomain> Items, int Total)> GetPagedByUsuarioIdAsync(
        Guid usuarioId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = DbSet
            .Include(d => d.Categoria)
            .Where(d => d.UsuarioId == usuarioId)
            .OrderBy(d => d.Tipo)
            .ThenBy(d => d.Nome);

        var total  = await query.CountAsync(ct);
        var models = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (Mapper.Map<IEnumerable<DespesaDomain>>(models), total);
    }

    public async Task<IEnumerable<DespesaDomain>> GetAtivosAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var models = await DbSet
            .Where(d => d.UsuarioId == usuarioId && d.Ativa)
            .OrderBy(d => d.Tipo)
            .ThenBy(d => d.Nome)
            .ToListAsync(ct);

        return Mapper.Map<IEnumerable<DespesaDomain>>(models);
    }

    public async Task<bool> UsuarioPossuiArquivoAsync(Guid arquivoId, Guid usuarioId, CancellationToken ct = default)
        => await DbSet.AnyAsync(d => d.ArquivoId == arquivoId && d.UsuarioId == usuarioId, ct);

    public async Task<Dictionary<Guid, Guid?>> GetArquivoIdsByIdsAsync(
        IEnumerable<Guid> despesaIds, CancellationToken ct = default)
    {
        var ids = despesaIds.Distinct().ToList();
        if (ids.Count == 0) return new Dictionary<Guid, Guid?>();

        return await DbSet
            .Where(d => ids.Contains(d.Id))
            .Select(d => new { d.Id, d.ArquivoId })
            .ToDictionaryAsync(x => x.Id, x => x.ArquivoId, ct);
    }
}
