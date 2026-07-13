using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockPersistanceLib.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Ticker = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RevGrowthScore = table.Column<double>(type: "REAL", nullable: true),
                    EpsGrowthScore = table.Column<double>(type: "REAL", nullable: true),
                    EvFcfScore = table.Column<double>(type: "REAL", nullable: true),
                    RoeRoicScore = table.Column<double>(type: "REAL", nullable: true),
                    EvEbitScore = table.Column<double>(type: "REAL", nullable: true),
                    NetworkEffectScore = table.Column<double>(type: "REAL", nullable: true),
                    CostAdvScore = table.Column<double>(type: "REAL", nullable: true),
                    SwitchCostScore = table.Column<double>(type: "REAL", nullable: true),
                    ScalabilityScore = table.Column<double>(type: "REAL", nullable: true),
                    IntangAssetScore = table.Column<double>(type: "REAL", nullable: true),
                    SectorGrowthScore = table.Column<double>(type: "REAL", nullable: true),
                    NonDisruptiveScore = table.Column<double>(type: "REAL", nullable: true),
                    ConsensusScore = table.Column<double>(type: "REAL", nullable: true),
                    MarginExpScore = table.Column<double>(type: "REAL", nullable: true),
                    MarketVolatilityScore = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YearlyFinancials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KeyFiguresJson = table.Column<string>(type: "TEXT", nullable: true),
                    StockId = table.Column<int>(type: "INTEGER", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEstimate = table.Column<bool>(type: "INTEGER", nullable: false),
                    Revenue = table.Column<double>(type: "REAL", nullable: false),
                    NmbrOfShares = table.Column<double>(type: "REAL", nullable: false),
                    StockPrice = table.Column<double>(type: "REAL", nullable: false),
                    OperatingCashFlow = table.Column<double>(type: "REAL", nullable: false),
                    Dividends = table.Column<double>(type: "REAL", nullable: false),
                    CapitalExpenditures = table.Column<double>(type: "REAL", nullable: false),
                    NetIncomeValue = table.Column<double>(type: "REAL", nullable: true),
                    EbitValue = table.Column<double>(type: "REAL", nullable: true),
                    EbitdaValue = table.Column<double>(type: "REAL", nullable: true),
                    Earnings_EbitdaMargin = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: true),
                    Earnings_EbitMargin = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: true),
                    Earnings_NetIncomeMargin = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: true),
                    MarketValue = table.Column<double>(type: "REAL", nullable: true),
                    LongTermDebt = table.Column<double>(type: "REAL", nullable: true),
                    ShortTermDebt = table.Column<double>(type: "REAL", nullable: true),
                    CashAndEquivalents = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearlyFinancials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YearlyFinancials_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YearlyFinancials_StockId_Year",
                table: "YearlyFinancials",
                columns: new[] { "StockId", "Year" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YearlyFinancials");

            migrationBuilder.DropTable(
                name: "Stocks");
        }
    }
}
