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
    public class ArquivoConfiguration : IEntityTypeConfiguration<ArquivoModel>
    {
        public void Configure(EntityTypeBuilder<ArquivoModel> builder)
        {
            builder.ToTable("arquivo", schema: "arquivo");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Nome)
                   .IsRequired()
                   .HasMaxLength(350);

            builder.Property(a => a.Extensao)
               .IsRequired(false)
               .HasMaxLength(20);

            builder.Property(a => a.ContentType)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(a => a.ArquivoByte)
               .IsRequired()
               .HasColumnType("bytea");

            builder.Property(a => a.DataCriacao)
               .IsRequired()
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(a => a.DataAlteracao)
                   .IsRequired()
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
