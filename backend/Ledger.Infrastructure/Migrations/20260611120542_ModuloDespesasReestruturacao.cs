using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModuloDespesasReestruturacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // -- 1. Criar tabela de categorias e popular com dados do sistema ------
            migrationBuilder.CreateTable(
                name: "categorias",
                schema: "ledger",
                columns: table => new
                {
                    id         = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    nome       = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    icone      = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    cor        = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_categorias", x => x.id); });

            migrationBuilder.InsertData(
                schema: "ledger",
                table: "categorias",
                columns: new[] { "id", "cor", "created_at", "icone", "nome", "updated_at", "usuario_id" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "#8B5CF6", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "pi-home",          "Moradia",        null, null },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "#3B82F6", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "pi-car",           "Transporte",     null, null },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "#F59E0B", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "pi-shopping-cart", "Alimenta��o",    null, null },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "#EF4444", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "pi-heart",         "Sa�de",          null, null },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "#06B6D4", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "pi-book",          "Educa��o",       null, null },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "#EC4899", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "pi-star",          "Lazer",          null, null },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "#10B981", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "pi-refresh",       "Assinaturas",    null, null },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "#F97316", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "pi-credit-card",   "Financiamentos", null, null },
                    { new Guid("00000000-0000-0000-0000-000000000099"), "#6B7280", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "pi-tag",           "Outros",         null, null }
                });

            // -- 2. Criar tabela de lan�amentos mensais ----------------------------
            migrationBuilder.CreateTable(
                name: "despesas_periodo",
                schema: "ledger",
                columns: table => new
                {
                    id             = table.Column<Guid>(type: "uuid", nullable: false),
                    despesa_id     = table.Column<Guid>(type: "uuid", nullable: true),
                    categoria_id   = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id     = table.Column<Guid>(type: "uuid", nullable: false),
                    descricao      = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    valor_planejado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    valor_realizado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    paga_em        = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    boleto_path    = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    competencia    = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at     = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at     = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_despesas_periodo", x => x.id);
                    table.ForeignKey(
                        name: "FK_despesas_periodo_categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalSchema: "ledger",
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_despesas_periodo_despesas_despesa_id",
                        column: x => x.despesa_id,
                        principalSchema: "ledger",
                        principalTable: "despesas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            // -- 3. Migrar despesas existentes para despesas_periodo (avulsas) -----
            migrationBuilder.Sql(@"
INSERT INTO ledger.despesas_periodo
    (id, despesa_id, categoria_id, usuario_id, descricao,
     valor_planejado, valor_realizado, paga_em, boleto_path, competencia, created_at, updated_at)
SELECT
    gen_random_uuid(),
    NULL,
    CASE categoria
        WHEN 1  THEN '00000000-0000-0000-0000-000000000001'::uuid  -- Moradia
        WHEN 2  THEN '00000000-0000-0000-0000-000000000003'::uuid  -- Alimenta��o
        WHEN 3  THEN '00000000-0000-0000-0000-000000000002'::uuid  -- Transporte
        WHEN 4  THEN '00000000-0000-0000-0000-000000000004'::uuid  -- Sa�de
        WHEN 5  THEN '00000000-0000-0000-0000-000000000005'::uuid  -- Educa��o
        WHEN 6  THEN '00000000-0000-0000-0000-000000000006'::uuid  -- Lazer
        WHEN 7  THEN '00000000-0000-0000-0000-000000000007'::uuid  -- Assinaturas
        ELSE         '00000000-0000-0000-0000-000000000099'::uuid  -- Outros
    END,
    usuario_id,
    descricao,
    valor,
    CASE WHEN paga THEN valor ELSE 0 END,
    CASE WHEN paga THEN data_pagamento ELSE NULL END,
    boleto_path,
    date_trunc('month', data_vencimento AT TIME ZONE 'UTC') AT TIME ZONE 'UTC',
    created_at,
    updated_at
FROM ledger.despesas;");

            // -- 4. Limpar despesas (templates come�am do zero) --------------------
            migrationBuilder.Sql("DELETE FROM ledger.despesas;");

            // -- 5. Remover �ndices antigos da tabela despesas ---------------------
            migrationBuilder.DropIndex(
                name: "IX_despesas_data_vencimento",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropIndex(
                name: "IX_despesas_usuario_id_categoria",
                schema: "ledger",
                table: "despesas");

            // -- 6. Remover colunas antigas ----------------------------------------
            migrationBuilder.DropColumn(name: "boleto_path",     schema: "ledger", table: "despesas");
            migrationBuilder.DropColumn(name: "categoria",       schema: "ledger", table: "despesas");
            migrationBuilder.DropColumn(name: "data_pagamento",  schema: "ledger", table: "despesas");
            migrationBuilder.DropColumn(name: "data_vencimento", schema: "ledger", table: "despesas");
            migrationBuilder.DropColumn(name: "paga",            schema: "ledger", table: "despesas");
            migrationBuilder.DropColumn(name: "recorrente",      schema: "ledger", table: "despesas");

            // -- 7. Renomear colunas -----------------------------------------------
            migrationBuilder.RenameColumn(name: "valor",    schema: "ledger", table: "despesas", newName: "valor_planejado");
            migrationBuilder.RenameColumn(name: "descricao",schema: "ledger", table: "despesas", newName: "nome");

            // -- 8. Adicionar novas colunas (tabela est� vazia, sem viola��o FK) ---
            migrationBuilder.AddColumn<bool>(
                name: "ativa", schema: "ledger", table: "despesas",
                type: "boolean", nullable: false, defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "categoria_id", schema: "ledger", table: "despesas",
                type: "uuid", nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000099")); // Outros

            migrationBuilder.AddColumn<int>(
                name: "dia_vencimento", schema: "ledger", table: "despesas",
                type: "integer", nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tipo", schema: "ledger", table: "despesas",
                type: "integer", nullable: false, defaultValue: 3); // Avulsa

            // -- 9. �ndices --------------------------------------------------------
            migrationBuilder.CreateIndex(name: "IX_despesas_categoria_id",     schema: "ledger", table: "despesas",        column:  "categoria_id");
            migrationBuilder.CreateIndex(name: "IX_despesas_usuario_id_ativa", schema: "ledger", table: "despesas",        columns: new[] { "usuario_id", "ativa" });
            migrationBuilder.CreateIndex(name: "IX_categorias_usuario_id",     schema: "ledger", table: "categorias",      column:  "usuario_id");
            migrationBuilder.CreateIndex(name: "IX_despesas_periodo_categoria_id",            schema: "ledger", table: "despesas_periodo", column:  "categoria_id");
            migrationBuilder.CreateIndex(name: "IX_despesas_periodo_despesa_id",              schema: "ledger", table: "despesas_periodo", column:  "despesa_id");
            migrationBuilder.CreateIndex(name: "IX_despesas_periodo_usuario_id_competencia",  schema: "ledger", table: "despesas_periodo", columns: new[] { "usuario_id", "competencia" });

            // -- 10. FK: despesas.categoria_id ? categorias.id --------------------
            migrationBuilder.AddForeignKey(
                name: "FK_despesas_categorias_categoria_id",
                schema: "ledger", table: "despesas",
                column: "categoria_id",
                principalSchema: "ledger", principalTable: "categorias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_despesas_categorias_categoria_id",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropTable(
                name: "despesas_periodo",
                schema: "ledger");

            migrationBuilder.DropTable(
                name: "categorias",
                schema: "ledger");

            migrationBuilder.DropIndex(
                name: "IX_despesas_categoria_id",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropIndex(
                name: "IX_despesas_usuario_id_ativa",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "ativa",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "categoria_id",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "dia_vencimento",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "tipo",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.RenameColumn(
                name: "valor_planejado",
                schema: "ledger",
                table: "despesas",
                newName: "valor");

            migrationBuilder.RenameColumn(
                name: "nome",
                schema: "ledger",
                table: "despesas",
                newName: "descricao");

            migrationBuilder.AddColumn<string>(
                name: "boleto_path",
                schema: "ledger",
                table: "despesas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "categoria",
                schema: "ledger",
                table: "despesas",
                type: "integer",
                nullable: false,
                defaultValue: 99);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_pagamento",
                schema: "ledger",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_vencimento",
                schema: "ledger",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "paga",
                schema: "ledger",
                table: "despesas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "recorrente",
                schema: "ledger",
                table: "despesas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_despesas_data_vencimento",
                schema: "ledger",
                table: "despesas",
                column: "data_vencimento");

            migrationBuilder.CreateIndex(
                name: "IX_despesas_usuario_id_categoria",
                schema: "ledger",
                table: "despesas",
                columns: new[] { "usuario_id", "categoria" });
        }
    }
}
