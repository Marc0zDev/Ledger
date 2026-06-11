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

    public async Task<IEnumerable<DespesaDomain>> GetAtivosAsync(Guid usuarioId, CancellationToken ct = default)
    {
        var models = await DbSet
            .Where(d => d.UsuarioId == usuarioId && d.Ativa)
            .OrderBy(d => d.Tipo)
            .ThenBy(d => d.Nome)
            .ToListAsync(ct);

        return Mapper.Map<IEnumerable<DespesaDomain>>(models);
    }
}
