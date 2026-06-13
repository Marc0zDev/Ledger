using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TemplateDateRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "data_fim",
                schema: "ledger",
                table: "receita_templates",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_inicio",
                schema: "ledger",
                table: "receita_templates",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "data_fim",
                schema: "ledger",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_inicio",
                schema: "ledger",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_fim",
                schema: "ledger",
                table: "receita_templates");

            migrationBuilder.DropColumn(
                name: "data_inicio",
                schema: "ledger",
                table: "receita_templates");

            migrationBuilder.DropColumn(
                name: "data_fim",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "data_inicio",
                schema: "ledger",
                table: "despesas");
        }
    }
}
