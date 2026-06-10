using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RedesenhoDespesasCategoriasCofre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_despesas_cofres_cofre_id",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropForeignKey(
                name: "FK_despesas_participantes_participante_id",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.RenameColumn(
                name: "participante_id",
                schema: "ledger",
                table: "despesas",
                newName: "usuario_id");

            migrationBuilder.RenameColumn(
                name: "data",
                schema: "ledger",
                table: "despesas",
                newName: "data_vencimento");

            migrationBuilder.RenameIndex(
                name: "IX_despesas_participante_id",
                schema: "ledger",
                table: "despesas",
                newName: "IX_despesas_usuario_id");

            migrationBuilder.AlterColumn<Guid>(
                name: "cofre_id",
                schema: "ledger",
                table: "despesas",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "boleto_path",
                schema: "ledger",
                table: "despesas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_pagamento",
                schema: "ledger",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "paga",
                schema: "ledger",
                table: "despesas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "categoria",
                schema: "ledger",
                table: "cofres",
                type: "integer",
                nullable: false,
                defaultValue: 99);

            migrationBuilder.CreateIndex(
                name: "IX_despesas_data_vencimento",
                schema: "ledger",
                table: "despesas",
                column: "data_vencimento");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_despesas_cofres_cofre_id",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropIndex(
                name: "IX_despesas_data_vencimento",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "boleto_path",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "data_pagamento",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "paga",
                schema: "ledger",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "categoria",
                schema: "ledger",
                table: "cofres");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                schema: "ledger",
                table: "despesas",
                newName: "participante_id");

            migrationBuilder.RenameColumn(
                name: "data_vencimento",
                schema: "ledger",
                table: "despesas",
                newName: "data");

            migrationBuilder.RenameIndex(
                name: "IX_despesas_usuario_id",
                schema: "ledger",
                table: "despesas",
                newName: "IX_despesas_participante_id");

            migrationBuilder.AlterColumn<Guid>(
                name: "cofre_id",
                schema: "ledger",
                table: "despesas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_despesas_cofres_cofre_id",
                schema: "ledger",
                table: "despesas",
                column: "cofre_id",
                principalSchema: "ledger",
                principalTable: "cofres",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_despesas_participantes_participante_id",
                schema: "ledger",
                table: "despesas",
                column: "participante_id",
                principalSchema: "ledger",
                principalTable: "participantes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
