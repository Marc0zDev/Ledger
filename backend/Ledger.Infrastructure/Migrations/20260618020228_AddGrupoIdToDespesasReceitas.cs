using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGrupoIdToDespesasReceitas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "grupo_id",
                schema: "ledger",
                table: "receitas",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "grupo_id",
                schema: "ledger",
                table: "despesas_periodo",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "grupo_id",
                schema: "ledger",
                table: "despesas",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_receitas_grupo_id",
                schema: "ledger",
                table: "receitas",
                column: "grupo_id");

            migrationBuilder.CreateIndex(
                name: "IX_despesas_periodo_grupo_id",
                schema: "ledger",
                table: "despesas_periodo",
                column: "grupo_id");

            migrationBuilder.CreateIndex(
                name: "IX_despesas_grupo_id",
                schema: "ledger",
                table: "despesas",
                column: "grupo_id");

            migrationBuilder.AddForeignKey(
                name: "FK_despesas_grupos_grupo_id",
                schema: "ledger",
                table: "despesas",
                column: "grupo_id",
                principalSchema: "ledger",
                principalTable: "grupos",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_despesas_periodo_grupos_grupo_id",
                schema: "ledger",
                table: "despesas_periodo",
                column: "grupo_id",
                principalSchema: "ledger",
                principalTable: "grupos",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_receitas_grupos_grupo_id",
                schema: "ledger",
                table: "receitas",
                column: "grupo_id",
                principalSchema: "ledger",
                principalTable: "grupos",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_despesas_grupos_grupo_id",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropForeignKey(
                name: "FK_despesas_periodo_grupos_grupo_id",
                schema: "ledger",
                table: "despesas_periodo");

            migrationBuilder.DropForeignKey(
                name: "FK_receitas_grupos_grupo_id",
                schema: "ledger",
                table: "receitas");

            migrationBuilder.DropIndex(
                name: "IX_receitas_grupo_id",
                schema: "ledger",
                table: "receitas");

            migrationBuilder.DropIndex(
                name: "IX_despesas_periodo_grupo_id",
                schema: "ledger",
                table: "despesas_periodo");

            migrationBuilder.DropIndex(
                name: "IX_despesas_grupo_id",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "grupo_id",
                schema: "ledger",
                table: "receitas");

            migrationBuilder.DropColumn(
                name: "grupo_id",
                schema: "ledger",
                table: "despesas_periodo");

            migrationBuilder.DropColumn(
                name: "grupo_id",
                schema: "ledger",
                table: "despesas");
        }
    }
}
