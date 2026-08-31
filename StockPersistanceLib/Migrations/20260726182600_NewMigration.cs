using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockPersistanceLib.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgentAnalysisJson",
                table: "Stocks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentAnalysisJson",
                table: "Stocks");
        }
    }
}
