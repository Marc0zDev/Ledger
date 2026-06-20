using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Infrastructure.Data.Configurations;

public class GrupoConfiguration : IEntityTypeConfiguration<GrupoModel>
{
    public void Configure(EntityTypeBuilder<GrupoModel> builder)
    {
        builder.ToTable("grupos", schema: "ledger");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
               .HasColumnName("id");

        builder.Property(g => g.Nome)
               .HasColumnName("nome")
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(g => g.Descricao)
               .HasColumnName("descricao")
               .HasMaxLength(500);

        builder.Property(g => g.CriadoPorUsuarioId)
               .HasColumnName("criado_por_usuario_id");

        builder.Property(g => g.CreatedAt)
               .HasColumnName("created_at")
               .HasColumnType("timestamp with time zone");

        builder.Property(g => g.UpdatedAt)
               .HasColumnName("updated_at")
               .HasColumnType("timestamp with time zone");

        builder.HasOne(g => g.CriadoPor)
               .WithMany()
               .HasForeignKey(g => g.CriadoPorUsuarioId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(g => g.Membros)
               .WithOne(m => m.Grupo)
               .HasForeignKey(m => m.GrupoId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
