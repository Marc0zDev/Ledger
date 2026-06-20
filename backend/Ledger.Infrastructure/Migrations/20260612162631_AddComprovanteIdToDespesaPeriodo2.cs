using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddComprovanteIdToDespesaPeriodo2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "comprovante_id",
                schema: "ledger",
                table: "despesas_periodo",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "comprovante_id",
                schema: "ledger",
                table: "despesas_periodo");
        }
    }
}
