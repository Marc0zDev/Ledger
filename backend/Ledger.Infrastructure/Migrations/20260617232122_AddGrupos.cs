using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGrupos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "grupos",
                schema: "ledger",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    criado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grupos", x => x.id);
                    table.ForeignKey(
                        name: "FK_grupos_usuarios_criado_por_usuario_id",
                        column: x => x.criado_por_usuario_id,
                        principalSchema: "auth",
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "grupo_membros",
                schema: "ledger",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    grupo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grupo_membros", x => x.id);
                    table.ForeignKey(
                        name: "FK_grupo_membros_grupos_grupo_id",
                        column: x => x.grupo_id,
                        principalSchema: "ledger",
                        principalTable: "grupos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_grupo_membros_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "auth",
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_grupo_membros_grupo_id_usuario_id",
                schema: "ledger",
                table: "grupo_membros",
                columns: new[] { "grupo_id", "usuario_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_grupo_membros_usuario_id",
                schema: "ledger",
                table: "grupo_membros",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_grupos_criado_por_usuario_id",
                schema: "ledger",
                table: "grupos",
                column: "criado_por_usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "grupo_membros",
                schema: "ledger");

            migrationBuilder.DropTable(
                name: "grupos",
                schema: "ledger");
        }
    }
}
