using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Infrastructure.Data.Configurations
{
    public class ReceitaConfiguration : IEntityTypeConfiguration<ReceitaModel>
    {
        public void Configure(EntityTypeBuilder<ReceitaModel> builder)
        {
            builder.ToTable("receitas", schema: "ledger");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Valor).IsRequired();

            builder.Property(r => r.Descricao)
                .HasMaxLength(500);

            builder.Property(r => r.DataRecebimento).IsRequired();
            builder.Property(r => r.DataCriacao).IsRequired();
            builder.Property(r => r.DataAtualizacao);

            builder.Property(r => r.UsuarioId).IsRequired().HasColumnName("usuario_id"); 
            builder.Property(r => r.ArquivoId).HasColumnName("arquivo_id");

            builder.HasOne<ArquivoModel>()
                .WithMany()
                .HasForeignKey(r => r.ArquivoId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
