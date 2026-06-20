using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Infrastructure.Data.Configurations;

public class GrupoMembroConfiguration : IEntityTypeConfiguration<GrupoMembroModel>
{
    public void Configure(EntityTypeBuilder<GrupoMembroModel> builder)
    {
        builder.ToTable("grupo_membros", schema: "ledger");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
               .HasColumnName("id");

        builder.Property(m => m.GrupoId)
               .HasColumnName("grupo_id");

        builder.Property(m => m.UsuarioId)
               .HasColumnName("usuario_id");

        builder.Property(m => m.Role)
               .HasColumnName("role");

        builder.Property(m => m.CreatedAt)
               .HasColumnName("created_at")
               .HasColumnType("timestamp with time zone");

        builder.Property(m => m.UpdatedAt)
               .HasColumnName("updated_at")
               .HasColumnType("timestamp with time zone");

        builder.HasOne(m => m.Usuario)
               .WithMany()
               .HasForeignKey(m => m.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.GrupoId, m.UsuarioId }).IsUnique();
    }
}
