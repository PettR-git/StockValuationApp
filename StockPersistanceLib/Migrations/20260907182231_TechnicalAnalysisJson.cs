using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockPersistanceLib.Migrations
{
    /// <inheritdoc />
    public partial class TechnicalAnalysisJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AgentAnalysisJson",
                table: "Stocks",
                newName: "TechnicalAnalysisAgentJson");

            migrationBuilder.AddColumn<string>(
                name: "OverviewAgentJson",
                table: "Stocks",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverviewAgentJson",
                table: "Stocks");

            migrationBuilder.RenameColumn(
                name: "TechnicalAnalysisAgentJson",
                table: "Stocks",
                newName: "AgentAnalysisJson");
        }
    }
}
