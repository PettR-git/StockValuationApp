using Microsoft.EntityFrameworkCore;
using StockLib.Main.Entities.Stocks;
using StockLib.Main.Entities.Stocks.Metrics;
using StockValuationApp.Entities.Enums;
using StockValuationApp.Entities.Stocks;
using StockValuationApp.Entities.Stocks.Metrics;
using System.Text.Json;

namespace StockPersistanceLib.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Stock> Stocks => Set<Stock>();
        public DbSet<YearlyFinancials> YearlyFinancials => Set<YearlyFinancials>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).HasMaxLength(200).IsRequired();
                entity.Property(s => s.Ticker).HasMaxLength(50).IsRequired();
                entity.Property(s => s.AgentAnalysisJson)
                      .HasColumnType("TEXT")
                      .IsRequired(false);
                entity.Property(s => s.LastPrice).HasColumnType("TEXT");
                entity.Property(s => s.LastUpdated).HasColumnType("TEXT");

                entity.HasMany(s => s.YearlyFinancials)
                    .WithOne()
                    .HasForeignKey(y => y.StockId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(s => s.WeeklyPrices)
                    .HasColumnName("WeeklyPricesJson")
                    .HasColumnType("TEXT")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => string.IsNullOrWhiteSpace(v)
                            ? Array.Empty<StockBar>()
                            : JsonSerializer.Deserialize<StockBar[]>(v, (JsonSerializerOptions?)null) ?? Array.Empty<StockBar>()
                    );

                entity.OwnsOne(s => s.StockScore, owned =>
                {
                    owned.Property(ss => ss.RevGrowthScore).HasColumnName("RevGrowthScore");
                    owned.Property(ss => ss.EpsGrowthScore).HasColumnName("EpsGrowthScore");
                    owned.Property(ss => ss.EvFcfScore).HasColumnName("EvFcfScore");
                    owned.Property(ss => ss.RoeRoicScore).HasColumnName("RoeRoicScore");
                    owned.Property(ss => ss.EvEbitScore).HasColumnName("EvEbitScore");
                    owned.Property(ss => ss.NetworkEffectScore).HasColumnName("NetworkEffectScore");
                    owned.Property(ss => ss.CostAdvScore).HasColumnName("CostAdvScore");
                    owned.Property(ss => ss.SwitchCostScore).HasColumnName("SwitchCostScore");
                    owned.Property(ss => ss.ScalabilityScore).HasColumnName("ScalabilityScore");
                    owned.Property(ss => ss.IntangAssetScore).HasColumnName("IntangAssetScore");
                    owned.Property(ss => ss.SectorGrowthScore).HasColumnName("SectorGrowthScore");
                    owned.Property(ss => ss.NonDisruptiveScore).HasColumnName("NonDisruptiveScore");
                    owned.Property(ss => ss.ConsensusScore).HasColumnName("ConsensusScore");
                    owned.Property(ss => ss.MarginExpScore).HasColumnName("MarginExpScore");
                    owned.Property(ss => ss.MarketVolatilityScore).HasColumnName("MarketVolatilityScore");
                });
            });

            modelBuilder.Entity<YearlyFinancials>(entity =>
            {
                entity.HasKey(y => y.Id);
                entity.Property(y => y.Year).IsRequired();
                entity.Property(y => y.Revenue).HasColumnType("REAL");
                entity.Property(y => y.NmbrOfShares).HasColumnType("REAL");
                entity.Property(y => y.StockPrice).HasColumnType("REAL");
                entity.Property(y => y.Dividends).HasColumnType("REAL");
                entity.Property(y => y.OperatingCashFlow).HasColumnType("REAL");
                entity.Property(y => y.CapitalExpenditures).HasColumnType("REAL");
                entity.Property(y => y.KeyFiguresJson).HasColumnType("TEXT");

                entity.OwnsOne(y => y.Earnings, owned =>
                {
                    owned.Property(e => e.NetIncomeValue).HasColumnName("NetIncomeValue").HasColumnType("REAL");
                    owned.Property(e => e.EbitValue).HasColumnName("EbitValue").HasColumnType("REAL");
                    owned.Property(e => e.EbitdaValue).HasColumnName("EbitdaValue").HasColumnType("REAL");
                    owned.Property(e => e.EbitdaMargin).HasColumnType("REAL");
                    owned.Property(e => e.EbitMargin).HasColumnType("REAL");
                    owned.Property(e => e.NetIncomeMargin).HasColumnType("REAL");
                });

                entity.OwnsOne(y => y.EnterpriseVal, owned =>
                {
                    owned.Property(e => e.MarketValue).HasColumnName("MarketValue").HasColumnType("REAL");
                    owned.Property(e => e.LongTermDebt).HasColumnName("LongTermDebt").HasColumnType("REAL");
                    owned.Property(e => e.ShortTermDebt).HasColumnName("ShortTermDebt").HasColumnType("REAL");
                    owned.Property(e => e.CashAndEquivalents).HasColumnName("CashAndEquivalents").HasColumnType("REAL");
                });

                entity.Property(y => y.KeyFiguresDict)
                    .HasColumnName("KeyFiguresJson") 
                    .HasColumnType("TEXT")
                    .HasConversion(
                        // When saving, make the Dictionary into a JSON string
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                        // When loading, make the JSON string back into a Dictionary
                        v => string.IsNullOrWhiteSpace(v)
                        ? new Dictionary<KeyFigureTypes, decimal>()
                        : System.Text.Json.JsonSerializer.Deserialize<Dictionary<KeyFigureTypes, decimal>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<KeyFigureTypes, decimal>()
                    );

                entity.HasIndex(y => new { y.StockId, y.Year }).IsUnique();
            });
        }
    }
}
