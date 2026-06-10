using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConvites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "convites",
                schema: "ledger",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cofre_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_convites", x => x.id);
                    table.ForeignKey(
                        name: "FK_convites_cofres_cofre_id",
                        column: x => x.cofre_id,
                        principalSchema: "ledger",
                        principalTable: "cofres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_convites_cofre_id",
                schema: "ledger",
                table: "convites",
                column: "cofre_id");

            migrationBuilder.CreateIndex(
                name: "IX_convites_status",
                schema: "ledger",
                table: "convites",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_convites_token",
                schema: "ledger",
                table: "convites",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_convites_usuario_id",
                schema: "ledger",
                table: "convites",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "convites",
                schema: "ledger");
        }
    }
}
