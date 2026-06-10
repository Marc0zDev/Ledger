using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Infrastructure.Data.Configurations;

public class CofreConfiguration : IEntityTypeConfiguration<CofreModel>
{
    public void Configure(EntityTypeBuilder<CofreModel> builder)
    {
        builder.ToTable("cofres", schema: "ledger");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .HasColumnName("id");

        builder.Property(c => c.Nome)
               .HasColumnName("nome")
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(c => c.Descricao)
               .HasColumnName("descricao");

        builder.Property(c => c.Meta)
               .HasColumnName("meta")
               .HasPrecision(18, 2);

        builder.Property(c => c.Status)
               .HasColumnName("status");

        builder.Property(c => c.Categoria)
               .HasColumnName("categoria")
               .HasDefaultValue(99);

        builder.Property(c => c.CriadoPorUsuarioId)
               .HasColumnName("criado_por_usuario_id");

        builder.Property(c => c.CreatedAt)
               .HasColumnName("created_at")
               .HasColumnType("timestamp with time zone");

        builder.Property(c => c.UpdatedAt)
               .HasColumnName("updated_at")
               .HasColumnType("timestamp with time zone");

        builder.HasMany(c => c.Participantes)
               .WithOne(p => p.Cofre)
               .HasForeignKey(p => p.CofreId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Movimentacoes)
               .WithOne(m => m.Cofre)
               .HasForeignKey(m => m.CofreId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
