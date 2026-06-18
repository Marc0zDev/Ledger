using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Infrastructure.Data.Configurations;

public class ConviteGrupoConfiguration : IEntityTypeConfiguration<ConviteGrupoModel>
{
    public void Configure(EntityTypeBuilder<ConviteGrupoModel> builder)
    {
        builder.ToTable("convite_grupos", "ledger");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.GrupoId)
            .HasColumnName("grupo_id")
            .IsRequired();

        builder.Property(c => c.ConvidadoPorUsuarioId)
            .HasColumnName("convidado_por_usuario_id")
            .IsRequired();

        builder.Property(c => c.UsuarioId)
            .HasColumnName("usuario_id")
            .IsRequired();

        builder.Property(c => c.Token)
            .HasColumnName("token")
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(c => c.Token).IsUnique();

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasDefaultValue(1);

        builder.Property(c => c.ExpiresAt).HasColumnName("expires_at");
        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(c => c.Grupo)
            .WithMany()
            .HasForeignKey(c => c.GrupoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.UsuarioId).HasDatabaseName("IX_convite_grupos_usuario_id");
        builder.HasIndex(c => c.GrupoId).HasDatabaseName("IX_convite_grupos_grupo_id");
        builder.HasIndex(c => c.Status).HasDatabaseName("IX_convite_grupos_status");
    }
}
