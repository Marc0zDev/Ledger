using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DespesasRemoveCofreAddCategoriaRecorrente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_despesas_cofres_cofre_id",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropIndex(
                name: "IX_despesas_cofre_id",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "cofre_id",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.AddColumn<int>(
                name: "categoria",
                schema: "ledger",
                table: "despesas",
                type: "integer",
                nullable: false,
                defaultValue: 99);

            migrationBuilder.AddColumn<bool>(
                name: "recorrente",
                schema: "ledger",
                table: "despesas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_despesas_usuario_id_categoria",
                schema: "ledger",
                table: "despesas",
                columns: new[] { "usuario_id", "categoria" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_despesas_usuario_id_categoria",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "categoria",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "recorrente",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.AddColumn<Guid>(
                name: "cofre_id",
                schema: "ledger",
                table: "despesas",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_despesas_cofre_id",
                schema: "ledger",
                table: "despesas",
                column: "cofre_id");

            migrationBuilder.AddForeignKey(
                name: "FK_despesas_cofres_cofre_id",
                schema: "ledger",
                table: "despesas",
                column: "cofre_id",
                principalSchema: "ledger",
                principalTable: "cofres",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
