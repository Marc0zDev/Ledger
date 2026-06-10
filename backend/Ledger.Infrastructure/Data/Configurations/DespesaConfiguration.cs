using Ledger.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Infrastructure.Data.Configurations;

public class DespesaConfiguration : IEntityTypeConfiguration<DespesaModel>
{
    public void Configure(EntityTypeBuilder<DespesaModel> builder)
    {
        builder.ToTable("despesas", schema: "ledger");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id).HasColumnName("id");
        builder.Property(d => d.Descricao).HasColumnName("descricao").IsRequired().HasMaxLength(200);
        builder.Property(d => d.Valor).HasColumnName("valor").HasPrecision(18, 2);
        builder.Property(d => d.DataVencimento).HasColumnName("data_vencimento").HasColumnType("timestamp with time zone");
        builder.Property(d => d.DataPagamento).HasColumnName("data_pagamento").HasColumnType("timestamp with time zone");
        builder.Property(d => d.Paga).HasColumnName("paga").HasDefaultValue(false);
        builder.Property(d => d.BoletoPath).HasColumnName("boleto_path").HasMaxLength(500);
        builder.Property(d => d.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(d => d.Categoria).HasColumnName("categoria").HasDefaultValue(99);
        builder.Property(d => d.Recorrente).HasColumnName("recorrente").HasDefaultValue(false);
        builder.Property(d => d.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone");
        builder.Property(d => d.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");

        builder.HasIndex(d => d.UsuarioId);
        builder.HasIndex(d => d.DataVencimento);
        builder.HasIndex(d => new { d.UsuarioId, d.Categoria });
    }
}
