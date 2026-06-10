using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Infrastructure.Data.Configurations;

public class ConviteConfiguration : IEntityTypeConfiguration<ConviteModel>
{
    public void Configure(EntityTypeBuilder<ConviteModel> builder)
    {
        builder.ToTable("convites", "ledger");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.CofreId)
            .HasColumnName("cofre_id")
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

        builder.HasOne(c => c.Cofre)
            .WithMany()
            .HasForeignKey(c => c.CofreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.UsuarioId).HasDatabaseName("IX_convites_usuario_id");
        builder.HasIndex(c => c.CofreId).HasDatabaseName("IX_convites_cofre_id");
        builder.HasIndex(c => c.Status).HasDatabaseName("IX_convites_status");
    }
}
