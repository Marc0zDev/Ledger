using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Infrastructure.Data.Configurations;

public class MovimentacaoConfiguration : IEntityTypeConfiguration<MovimentacaoModel>
{
    public void Configure(EntityTypeBuilder<MovimentacaoModel> builder)
    {
        builder.ToTable("movimentacoes", schema: "ledger");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
               .HasColumnName("id");

        builder.Property(m => m.Descricao)
               .HasColumnName("descricao")
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(m => m.Valor)
               .HasColumnName("valor")
               .HasPrecision(18, 2);

        builder.Property(m => m.Tipo)
               .HasColumnName("tipo");

        builder.Property(m => m.Status)
               .HasColumnName("status")
               .IsRequired()
               .HasDefaultValue(1);

        builder.Property(m => m.Data)
               .HasColumnName("data")
               .HasColumnType("timestamp with time zone");

        builder.Property(m => m.CofreId)
               .HasColumnName("cofre_id");

        builder.Property(m => m.UsuarioId)
               .HasColumnName("usuario_id");

        builder.Property(m => m.CreatedAt)
               .HasColumnName("created_at")
               .HasColumnType("timestamp with time zone");

        builder.Property(m => m.UpdatedAt)
               .HasColumnName("updated_at")
               .HasColumnType("timestamp with time zone");

        builder.HasOne(m => m.Cofre)
               .WithMany(c => c.Movimentacoes)
               .HasForeignKey(m => m.CofreId)
               .OnDelete(DeleteBehavior.Cascade);

        // FK cross-schema: movimentacao → auth.usuarios
        builder.HasOne(m => m.Usuario)
               .WithMany()
               .HasForeignKey(m => m.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
