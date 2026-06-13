using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReceita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ledge");

            migrationBuilder.CreateTable(
                name: "receitas",
                schema: "ledge",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    arquivo_id = table.Column<Guid>(type: "uuid", nullable: true),
                    DataRecebimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ArquivoId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    UsuarioId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receitas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_receitas_arquivo_ArquivoId1",
                        column: x => x.ArquivoId1,
                        principalSchema: "arquivo",
                        principalTable: "arquivo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_receitas_arquivo_arquivo_id",
                        column: x => x.arquivo_id,
                        principalSchema: "arquivo",
                        principalTable: "arquivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_receitas_usuarios_UsuarioId1",
                        column: x => x.UsuarioId1,
                        principalSchema: "auth",
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_receitas_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "auth",
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_receitas_arquivo_id",
                schema: "ledge",
                table: "receitas",
                column: "arquivo_id");

            migrationBuilder.CreateIndex(
                name: "IX_receitas_ArquivoId1",
                schema: "ledge",
                table: "receitas",
                column: "ArquivoId1");

            migrationBuilder.CreateIndex(
                name: "IX_receitas_usuario_id",
                schema: "ledge",
                table: "receitas",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_receitas_UsuarioId1",
                schema: "ledge",
                table: "receitas",
                column: "UsuarioId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "receitas",
                schema: "ledge");
        }
    }
}
