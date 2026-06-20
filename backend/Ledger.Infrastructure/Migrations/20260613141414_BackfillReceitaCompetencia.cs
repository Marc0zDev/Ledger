using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BackfillReceitaCompetencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Backfill: set competencia = first day of the month of DataRecebimento for rows with -infinity
            migrationBuilder.Sql(@"
                UPDATE ledger.receitas
                SET competencia = DATE_TRUNC('month', ""DataRecebimento"") AT TIME ZONE 'UTC'
                WHERE competencia = '-infinity'::timestamptz
                   OR competencia < '2000-01-01'::timestamptz;
            ");

            // Remove the problematic default so new rows must always provide competencia
            migrationBuilder.Sql(@"
                ALTER TABLE ledger.receitas ALTER COLUMN competencia DROP DEFAULT;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ledger.receitas ALTER COLUMN competencia SET DEFAULT TIMESTAMPTZ '-infinity';
            ");
        }
    }
}
