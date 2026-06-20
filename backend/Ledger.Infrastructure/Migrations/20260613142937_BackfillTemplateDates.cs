using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ledger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BackfillTemplateDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ledger.despesas
                SET data_inicio = DATE_TRUNC('month', created_at) AT TIME ZONE 'UTC'
                WHERE data_inicio = '-infinity'::timestamptz OR data_inicio < '2000-01-01'::timestamptz;
            ");
            migrationBuilder.Sql(@"
                UPDATE ledger.receita_templates
                SET data_inicio = DATE_TRUNC('month', ""CreatedAt"") AT TIME ZONE 'UTC'
                WHERE data_inicio = '-infinity'::timestamptz OR data_inicio < '2000-01-01'::timestamptz;
            ");
            migrationBuilder.Sql(@"ALTER TABLE ledger.despesas ALTER COLUMN data_inicio DROP DEFAULT;");
            migrationBuilder.Sql(@"ALTER TABLE ledger.receita_templates ALTER COLUMN data_inicio DROP DEFAULT;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ledger.despesas ALTER COLUMN data_inicio SET DEFAULT TIMESTAMPTZ '-infinity';");
            migrationBuilder.Sql(@"ALTER TABLE ledger.receita_templates ALTER COLUMN data_inicio SET DEFAULT TIMESTAMPTZ '-infinity';");
        }
    }
}
