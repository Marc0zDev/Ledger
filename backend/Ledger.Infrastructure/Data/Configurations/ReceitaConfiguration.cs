using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Infrastructure.Data.Configurations;

public class ReceitaConfiguration : IEntityTypeConfiguration<ReceitaModel>
{
    public void Configure(EntityTypeBuilder<ReceitaModel> builder)
    {
        builder.ToTable("receitas", schema: "ledger");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Nome).IsRequired().HasMaxLength(100);
        builder.Property(r => r.Valor).IsRequired();
        builder.Property(r => r.Descricao).HasMaxLength(500);
        builder.Property(r => r.DataRecebimento).IsRequired();
        builder.Property(r => r.Competencia).IsRequired().HasColumnName("competencia");
        builder.Property(r => r.CreatedAt).IsRequired().HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");
        builder.Property(r => r.UsuarioId).IsRequired().HasColumnName("usuario_id");
        builder.Property(r => r.ArquivoId).HasColumnName("arquivo_id");
        builder.Property(r => r.ReceitaTemplateId).HasColumnName("receita_template_id");

        builder.HasOne(r => r.Arquivo)
            .WithMany()
            .HasForeignKey(r => r.ArquivoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Usuario)
            .WithMany()
            .HasForeignKey(r => r.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Template)
            .WithMany()
            .HasForeignKey(r => r.ReceitaTemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(r => r.GrupoId).HasColumnName("grupo_id");
        builder.HasOne(r => r.Grupo)
            .WithMany()
            .HasForeignKey(r => r.GrupoId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(r => r.GrupoId);
    }
}
