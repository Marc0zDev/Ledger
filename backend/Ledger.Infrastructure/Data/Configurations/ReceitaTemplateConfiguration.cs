using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Infrastructure.Data.Configurations;

public class ReceitaTemplateConfiguration : IEntityTypeConfiguration<ReceitaTemplateModel>
{
    public void Configure(EntityTypeBuilder<ReceitaTemplateModel> builder)
    {
        builder.ToTable("receita_templates", schema: "ledger");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Nome).IsRequired().HasMaxLength(100);
        builder.Property(r => r.Valor).IsRequired();
        builder.Property(r => r.Descricao).HasMaxLength(500);
        builder.Property(r => r.Ativa).IsRequired();
        builder.Property(r => r.DataInicio).IsRequired().HasColumnName("data_inicio").HasColumnType("timestamp with time zone");
        builder.Property(r => r.DataFim).HasColumnName("data_fim").HasColumnType("timestamp with time zone");
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.Property(r => r.UsuarioId).IsRequired().HasColumnName("usuario_id");

        builder.HasOne(r => r.Usuario)
            .WithMany()
            .HasForeignKey(r => r.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
