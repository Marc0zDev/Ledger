using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConviteGrupos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "convite_grupos",
                schema: "ledger",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    grupo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    convidado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_convite_grupos", x => x.id);
                    table.ForeignKey(
                        name: "FK_convite_grupos_grupos_grupo_id",
                        column: x => x.grupo_id,
                        principalSchema: "ledger",
                        principalTable: "grupos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_convite_grupos_grupo_id",
                schema: "ledger",
                table: "convite_grupos",
                column: "grupo_id");

            migrationBuilder.CreateIndex(
                name: "IX_convite_grupos_status",
                schema: "ledger",
                table: "convite_grupos",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_convite_grupos_token",
                schema: "ledger",
                table: "convite_grupos",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_convite_grupos_usuario_id",
                schema: "ledger",
                table: "convite_grupos",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "convite_grupos",
                schema: "ledger");
        }
    }
}
