using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArquivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "arquivo");

            migrationBuilder.AddColumn<Guid>(
                name: "arquivo_id",
                schema: "ledger",
                table: "despesas",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "arquivo",
                schema: "arquivo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Extensao = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ArquivoByte = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_arquivo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_despesas_arquivo_id",
                schema: "ledger",
                table: "despesas",
                column: "arquivo_id");

            migrationBuilder.AddForeignKey(
                name: "FK_despesas_arquivo_arquivo_id",
                schema: "ledger",
                table: "despesas",
                column: "arquivo_id",
                principalSchema: "arquivo",
                principalTable: "arquivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_despesas_arquivo_arquivo_id",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropTable(
                name: "arquivo",
                schema: "arquivo");

            migrationBuilder.DropIndex(
                name: "IX_despesas_arquivo_id",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "arquivo_id",
                schema: "ledger",
                table: "despesas");
        }
    }
}
