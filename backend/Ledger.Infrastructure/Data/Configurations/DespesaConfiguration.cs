using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Infrastructure.Data.Configurations;

public class DespesaConfiguration : IEntityTypeConfiguration<DespesaModel>
{
    public void Configure(EntityTypeBuilder<DespesaModel> builder)
    {
        builder.ToTable("despesas", schema: "ledger");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id).HasColumnName("id");
        builder.Property(d => d.Nome).HasColumnName("nome").IsRequired().HasMaxLength(200);
        builder.Property(d => d.Tipo).HasColumnName("tipo").IsRequired();
        builder.Property(d => d.ValorPlanejado).HasColumnName("valor_planejado").HasPrecision(18, 2);
        builder.Property(d => d.DiaVencimento).HasColumnName("dia_vencimento");
        builder.Property(d => d.Ativa).HasColumnName("ativa").HasDefaultValue(true);
        builder.Property(d => d.CategoriaId).HasColumnName("categoria_id").IsRequired();
        builder.Property(d => d.ArquivoId).HasColumnName("arquivo_id").IsRequired(false);
        builder.Property(d => d.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(d => d.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone");
        builder.Property(d => d.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");

        builder.HasOne(d => d.Categoria)
               .WithMany()
               .HasForeignKey(d => d.CategoriaId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Arquivo)
               .WithMany()
               .HasForeignKey(d => d.ArquivoId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(d => d.UsuarioId);
        builder.HasIndex(d => new { d.UsuarioId, d.Ativa });
    }
}
