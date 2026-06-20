using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReceitaTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_receitas_arquivo_ArquivoId1",
                schema: "ledger",
                table: "receitas");

            migrationBuilder.RenameColumn(
                name: "DataCriacao",
                schema: "ledger",
                table: "receitas",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "DataAtualizacao",
                schema: "ledger",
                table: "receitas",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ArquivoId1",
                schema: "ledger",
                table: "receitas",
                newName: "receita_template_id");

            migrationBuilder.RenameIndex(
                name: "IX_receitas_ArquivoId1",
                schema: "ledger",
                table: "receitas",
                newName: "IX_receitas_receita_template_id");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                schema: "ledger",
                table: "receitas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<DateTime>(
                name: "competencia",
                schema: "ledger",
                table: "receitas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "receita_templates",
                schema: "ledger",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ativa = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receita_templates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_receita_templates_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "auth",
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_receita_templates_usuario_id",
                schema: "ledger",
                table: "receita_templates",
                column: "usuario_id");

            migrationBuilder.AddForeignKey(
                name: "FK_receitas_receita_templates_receita_template_id",
                schema: "ledger",
                table: "receitas",
                column: "receita_template_id",
                principalSchema: "ledger",
                principalTable: "receita_templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_receitas_receita_templates_receita_template_id",
                schema: "ledger",
                table: "receitas");

            migrationBuilder.DropTable(
                name: "receita_templates",
                schema: "ledger");

            migrationBuilder.DropColumn(
                name: "competencia",
                schema: "ledger",
                table: "receitas");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "ledger",
                table: "receitas",
                newName: "DataAtualizacao");

            migrationBuilder.RenameColumn(
                name: "receita_template_id",
                schema: "ledger",
                table: "receitas",
                newName: "ArquivoId1");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "ledger",
                table: "receitas",
                newName: "DataCriacao");

            migrationBuilder.RenameIndex(
                name: "IX_receitas_receita_template_id",
                schema: "ledger",
                table: "receitas",
                newName: "IX_receitas_ArquivoId1");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                schema: "ledger",
                table: "receitas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_receitas_arquivo_ArquivoId1",
                schema: "ledger",
                table: "receitas",
                column: "ArquivoId1",
                principalSchema: "arquivo",
                principalTable: "arquivo",
                principalColumn: "Id");
        }
    }
}
