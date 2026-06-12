using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Infrastructure.Data.Configurations;

public class DespesaPeriodoConfiguration : IEntityTypeConfiguration<DespesaPeriodoModel>
{
    public void Configure(EntityTypeBuilder<DespesaPeriodoModel> builder)
    {
        builder.ToTable("despesas_periodo", schema: "ledger");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id).HasColumnName("id");
        builder.Property(d => d.DespesaId).HasColumnName("despesa_id");
        builder.Property(d => d.CategoriaId).HasColumnName("categoria_id").IsRequired();
        builder.Property(d => d.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(d => d.Descricao).HasColumnName("descricao").IsRequired().HasMaxLength(200);
        builder.Property(d => d.ValorPlanejado).HasColumnName("valor_planejado").HasPrecision(18, 2);
        builder.Property(d => d.ValorRealizado).HasColumnName("valor_realizado").HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(d => d.PagaEm).HasColumnName("paga_em").HasColumnType("timestamp with time zone");
        builder.Property(d => d.BoletoPath).HasColumnName("boleto_path").HasMaxLength(500);
        builder.Property(d => d.ComprovanteId).HasColumnName("comprovante_id");
        builder.Property(d => d.Competencia).HasColumnName("competencia").HasColumnType("timestamp with time zone");
        builder.Property(d => d.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone");
        builder.Property(d => d.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");

        builder.HasOne(d => d.Despesa)
               .WithMany()
               .HasForeignKey(d => d.DespesaId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(d => d.Categoria)
               .WithMany()
               .HasForeignKey(d => d.CategoriaId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => new { d.UsuarioId, d.Competencia });
        builder.HasIndex(d => d.DespesaId).IsUnique(false);
    }
}
