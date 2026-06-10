using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Infrastructure.Data.Configurations;

public class ParticipanteConfiguration : IEntityTypeConfiguration<ParticipanteModel>
{
    public void Configure(EntityTypeBuilder<ParticipanteModel> builder)
    {
        builder.ToTable("participantes", schema: "ledger");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
               .HasColumnName("id");

        builder.Property(p => p.CofreId)
               .HasColumnName("cofre_id");

        builder.Property(p => p.UsuarioId)
               .HasColumnName("usuario_id");

        builder.Property(p => p.CreatedAt)
               .HasColumnName("created_at")
               .HasColumnType("timestamp with time zone");

        builder.Property(p => p.UpdatedAt)
               .HasColumnName("updated_at")
               .HasColumnType("timestamp with time zone");

        // Índice único: um usuário só pode estar uma vez por cofre
        builder.HasIndex(p => new { p.CofreId, p.UsuarioId }).IsUnique();

        // FK cross-schema: participante → auth.usuarios (ApplicationUser)
        builder.HasOne(p => p.Usuario)
               .WithMany()
               .HasForeignKey(p => p.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
