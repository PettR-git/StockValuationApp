using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockPersistanceLib.Migrations
{
    /// <inheritdoc />
    public partial class AndraPrecisionTillReal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Earnings_NetIncomeMargin",
                table: "YearlyFinancials",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Earnings_EbitdaMargin",
                table: "YearlyFinancials",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Earnings_EbitMargin",
                table: "YearlyFinancials",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Earnings_NetIncomeMargin",
                table: "YearlyFinancials",
                type: "TEXT",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Earnings_EbitdaMargin",
                table: "YearlyFinancials",
                type: "TEXT",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Earnings_EbitMargin",
                table: "YearlyFinancials",
                type: "TEXT",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "REAL",
                oldNullable: true);
        }
    }
}
