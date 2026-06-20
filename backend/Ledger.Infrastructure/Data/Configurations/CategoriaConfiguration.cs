using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Infrastructure.Data.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<CategoriaModel>
{
    // GUIDs estáveis para categorias do sistema (não mudam entre ambientes)
    public static readonly Guid IdMoradia        = new("00000000-0000-0000-0000-000000000001");
    public static readonly Guid IdTransporte      = new("00000000-0000-0000-0000-000000000002");
    public static readonly Guid IdAlimentacao     = new("00000000-0000-0000-0000-000000000003");
    public static readonly Guid IdSaude           = new("00000000-0000-0000-0000-000000000004");
    public static readonly Guid IdEducacao        = new("00000000-0000-0000-0000-000000000005");
    public static readonly Guid IdLazer           = new("00000000-0000-0000-0000-000000000006");
    public static readonly Guid IdAssinaturas     = new("00000000-0000-0000-0000-000000000007");
    public static readonly Guid IdFinanciamentos  = new("00000000-0000-0000-0000-000000000008");
    public static readonly Guid IdOutros          = new("00000000-0000-0000-0000-000000000099");

    public void Configure(EntityTypeBuilder<CategoriaModel> builder)
    {
        builder.ToTable("categorias", schema: "ledger");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.UsuarioId).HasColumnName("usuario_id");
        builder.Property(c => c.Nome).HasColumnName("nome").IsRequired().HasMaxLength(100);
        builder.Property(c => c.Icone).HasColumnName("icone").HasMaxLength(50);
        builder.Property(c => c.Cor).HasColumnName("cor").HasMaxLength(20);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");

        builder.HasIndex(c => c.UsuarioId);

        // ── Seed: categorias do sistema ───────────────────────────────────
        var seed = DateTime.SpecifyKind(new DateTime(2026, 1, 1), DateTimeKind.Utc);
        builder.HasData(
            new CategoriaModel { Id = IdMoradia,       UsuarioId = null, Nome = "Moradia",        Icone = "pi-home",          Cor = "#8B5CF6", CreatedAt = seed },
            new CategoriaModel { Id = IdTransporte,    UsuarioId = null, Nome = "Transporte",     Icone = "pi-car",           Cor = "#3B82F6", CreatedAt = seed },
            new CategoriaModel { Id = IdAlimentacao,   UsuarioId = null, Nome = "Alimentação",    Icone = "pi-shopping-cart", Cor = "#F59E0B", CreatedAt = seed },
            new CategoriaModel { Id = IdSaude,         UsuarioId = null, Nome = "Saúde",          Icone = "pi-heart",         Cor = "#EF4444", CreatedAt = seed },
            new CategoriaModel { Id = IdEducacao,      UsuarioId = null, Nome = "Educação",       Icone = "pi-book",          Cor = "#06B6D4", CreatedAt = seed },
            new CategoriaModel { Id = IdLazer,         UsuarioId = null, Nome = "Lazer",          Icone = "pi-star",          Cor = "#EC4899", CreatedAt = seed },
            new CategoriaModel { Id = IdAssinaturas,   UsuarioId = null, Nome = "Assinaturas",    Icone = "pi-refresh",       Cor = "#10B981", CreatedAt = seed },
            new CategoriaModel { Id = IdFinanciamentos,UsuarioId = null, Nome = "Financiamentos", Icone = "pi-credit-card",   Cor = "#F97316", CreatedAt = seed },
            new CategoriaModel { Id = IdOutros,        UsuarioId = null, Nome = "Outros",         Icone = "pi-tag",           Cor = "#6B7280", CreatedAt = seed }
        );
    }
}
