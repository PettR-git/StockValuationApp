using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockPersistanceLib.Migrations
{
    /// <inheritdoc />
    public partial class KeyMetricMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "KeyFiguresJson",
                table: "YearlyFinancials",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyFiguresJson1",
                table: "YearlyFinancials",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyFiguresJson1",
                table: "YearlyFinancials");

            migrationBuilder.AlterColumn<string>(
                name: "KeyFiguresJson",
                table: "YearlyFinancials",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
