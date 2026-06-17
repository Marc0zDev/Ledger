using Ledger.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Data;

public class LedgerDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public LedgerDbContext(DbContextOptions<LedgerDbContext> options) : base(options) { }

    // ── Ledger tables ─────────────────────────────────────────────────────
    public DbSet<CofreModel> Cofres => Set<CofreModel>();
    public DbSet<ParticipanteModel> Participantes => Set<ParticipanteModel>();
    public DbSet<CategoriaModel> Categorias => Set<CategoriaModel>();
    public DbSet<DespesaModel> Despesas => Set<DespesaModel>();
    public DbSet<DespesaPeriodoModel> DespesasPeriodo => Set<DespesaPeriodoModel>();
    public DbSet<MovimentacaoModel>  Movimentacoes   => Set<MovimentacaoModel>();
    public DbSet<ConviteModel> Convites => Set<ConviteModel>();
    public DbSet<ArquivoModel> Arquivos => Set<ArquivoModel>();
    public DbSet<ReceitaModel> Receitas => Set<ReceitaModel>();
    public DbSet<ReceitaTemplateModel> ReceitaTemplates => Set<ReceitaTemplateModel>();
    public DbSet<GrupoModel> Grupos => Set<GrupoModel>();
    public DbSet<GrupoMembroModel> GrupoMembros => Set<GrupoMembroModel>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Schema "auth": tabelas do Identity ────────────────────────────
        builder.Entity<ApplicationUser>()
               .ToTable("usuarios", schema: "auth");
        builder.Entity<IdentityRole<Guid>>()
               .ToTable("roles", schema: "auth");
        builder.Entity<IdentityUserRole<Guid>>()
               .ToTable("usuario_roles", schema: "auth");
        builder.Entity<IdentityUserClaim<Guid>>()
               .ToTable("usuario_claims", schema: "auth");
        builder.Entity<IdentityUserLogin<Guid>>()
               .ToTable("usuario_logins", schema: "auth");
        builder.Entity<IdentityRoleClaim<Guid>>()
               .ToTable("role_claims", schema: "auth");
        builder.Entity<IdentityUserToken<Guid>>()
               .ToTable("usuario_tokens", schema: "auth");

        // ── Schema "ledger": configurações por arquivo ────────────────────
        builder.ApplyConfigurationsFromAssembly(typeof(LedgerDbContext).Assembly);
    }
}
