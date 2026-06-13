using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReceita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_receitas_usuarios_UsuarioId1",
                schema: "ledge",
                table: "receitas");

            migrationBuilder.DropIndex(
                name: "IX_receitas_UsuarioId1",
                schema: "ledge",
                table: "receitas");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                schema: "ledge",
                table: "receitas");

            migrationBuilder.RenameTable(
                name: "receitas",
                schema: "ledge",
                newName: "receitas",
                newSchema: "ledger");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ledge");

            migrationBuilder.RenameTable(
                name: "receitas",
                schema: "ledger",
                newName: "receitas",
                newSchema: "ledge");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId1",
                schema: "ledge",
                table: "receitas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_receitas_UsuarioId1",
                schema: "ledge",
                table: "receitas",
                column: "UsuarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_receitas_usuarios_UsuarioId1",
                schema: "ledge",
                table: "receitas",
                column: "UsuarioId1",
                principalSchema: "auth",
                principalTable: "usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
