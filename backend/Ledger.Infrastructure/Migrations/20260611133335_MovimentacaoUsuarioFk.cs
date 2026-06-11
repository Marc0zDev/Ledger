using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MovimentacaoUsuarioFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_movimentacoes_usuario_id",
                schema: "ledger",
                table: "movimentacoes",
                column: "usuario_id");

            migrationBuilder.AddForeignKey(
                name: "FK_movimentacoes_usuarios_usuario_id",
                schema: "ledger",
                table: "movimentacoes",
                column: "usuario_id",
                principalSchema: "auth",
                principalTable: "usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_movimentacoes_usuarios_usuario_id",
                schema: "ledger",
                table: "movimentacoes");

            migrationBuilder.DropIndex(
                name: "IX_movimentacoes_usuario_id",
                schema: "ledger",
                table: "movimentacoes");
        }
    }
}
