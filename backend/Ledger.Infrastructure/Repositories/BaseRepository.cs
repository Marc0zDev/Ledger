using AutoMapper;
using Ledger.Domain.Base;
using Ledger.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories;

/// <summary>
/// Implementação genérica de repositório usando EF Core.
/// TDomain = entidade de domínio | TModel = modelo de persistência EF.
/// O mapeamento entre os dois é feito pelo AutoMapper via InfrastructureProfile.
/// </summary>
public abstract class BaseRepository<TDomain, TModel> : IRepository<TDomain>
    where TDomain : BaseDomain
    where TModel : class
{
    protected readonly DbContext Context;
    protected readonly DbSet<TModel> DbSet;
    protected readonly IMapper Mapper;

    protected BaseRepository(DbContext context, IMapper mapper)
    {
        Context = context;
        DbSet = context.Set<TModel>();
        Mapper = mapper;
    }

    public virtual async Task<TDomain?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var model = await DbSet.FindAsync(new object[] { id }, ct);
        return model is null ? null : Mapper.Map<TDomain>(model);
    }

    public virtual async Task<IEnumerable<TDomain>> GetAllAsync(CancellationToken ct = default)
    {
        var models = await DbSet.AsNoTracking().ToListAsync(ct);
        return Mapper.Map<IEnumerable<TDomain>>(models);
    }

    public virtual async Task AddAsync(TDomain entity, CancellationToken ct = default)
    {
        await DbSet.AddAsync(Mapper.Map<TModel>(entity), ct);
        await Context.SaveChangesAsync(ct);
    }

    public virtual async Task UpdateAsync(TDomain entity, CancellationToken ct = default)
    {
        DbSet.Update(Mapper.Map<TModel>(entity));
        await Context.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var model = await DbSet.FindAsync(new object[] { id }, ct);
        if (model is not null)
        {
            DbSet.Remove(model);
            await Context.SaveChangesAsync(ct);
        }
    }
}
