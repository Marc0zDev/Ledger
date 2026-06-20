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

    // ── Transação ──────────────────────────────────────────────────────────────
    // Se já existe uma transação ativa (iniciada externamente), apenas executa
    // a operação dentro dela. Caso contrário, abre, commita ou faz rollback.
    private async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken ct)
    {
        if (Context.Database.CurrentTransaction is not null)
        {
            await operation();
            return;
        }

        await using var transaction = await Context.Database.BeginTransactionAsync(ct);
        try
        {
            await operation();
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    // ── Leitura ────────────────────────────────────────────────────────────────

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

    // ── Escrita ────────────────────────────────────────────────────────────────

    public virtual async Task AddAsync(TDomain entity, CancellationToken ct = default)
    {
        await ExecuteInTransactionAsync(async () =>
        {
            await DbSet.AddAsync(Mapper.Map<TModel>(entity), ct);
            await Context.SaveChangesAsync(ct);
        }, ct);
    }

    public virtual async Task UpdateAsync(TDomain entity, CancellationToken ct = default)
    {
        await ExecuteInTransactionAsync(async () =>
        {
            var newModel = Mapper.Map<TModel>(entity);

            var tracked = Context.ChangeTracker.Entries<TModel>()
                .FirstOrDefault(e => (Guid)e.Property("Id").CurrentValue! == entity.Id);
            if (tracked is not null)
                tracked.State = EntityState.Detached;

            DbSet.Update(newModel);
            await Context.SaveChangesAsync(ct);
        }, ct);
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await ExecuteInTransactionAsync(async () =>
        {
            var model = await DbSet.FindAsync(new object[] { id }, ct);
            if (model is not null)
            {
                DbSet.Remove(model);
                await Context.SaveChangesAsync(ct);
            }
        }, ct);
    }
}

