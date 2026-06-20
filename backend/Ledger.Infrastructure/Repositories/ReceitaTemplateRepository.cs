using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using Ledger.Infrastructure.Data;
using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories;

public class ReceitaTemplateRepository : BaseRepository<ReceitaTemplateDomain, ReceitaTemplateModel>, IReceitaTemplateRepository
{
    public ReceitaTemplateRepository(LedgerDbContext context, IMapper mapper) : base(context, mapper) { }

    public async Task<IEnumerable<ReceitaTemplateDomain>> GetByUsuarioIdAsync(Guid usuarioId, CancellationToken ct)
    {
        var models = await DbSet.AsNoTracking()
            .Where(r => r.UsuarioId == usuarioId)
            .OrderBy(r => r.Nome)
            .ToListAsync(ct);
        return Mapper.Map<IEnumerable<ReceitaTemplateDomain>>(models);
    }

    public async Task<IEnumerable<ReceitaTemplateDomain>> GetAtivosAsync(Guid usuarioId, CancellationToken ct)
    {
        var models = await DbSet.AsNoTracking()
            .Where(r => r.UsuarioId == usuarioId && r.Ativa)
            .OrderBy(r => r.Nome)
            .ToListAsync(ct);
        return Mapper.Map<IEnumerable<ReceitaTemplateDomain>>(models);
    }
}
