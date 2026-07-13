using Microsoft.EntityFrameworkCore;
using StockLib.Abstraction;
using StockLib.Main.Entities.Stocks;
using StockPersistanceLib.Data;
using StockValuationApp.Entities.Enums;
using StockValuationApp.Entities.Stocks;
using StockValuationApp.Entities.Stocks.Metrics;
using StockValuationApp.Entities.Stocks.Metrics.Earnings;


namespace StockPersistanceLib.Repositories
{
    public class EfStockRepository : IStockRepository
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public EfStockRepository(IDbContextFactory<AppDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Stock>> LoadAllAsync()
        {
            await using var context = _factory.CreateDbContext();

            return await context.Stocks
                .AsNoTracking()
                .Include(s => s.Financials)
                .ToListAsync();
        }

        public async Task SaveAllAsync(IEnumerable<Stock> stocks)
        {
            var incomingStocks = (stocks ?? Enumerable.Empty<Stock>()).ToList();

            await using var context = _factory.CreateDbContext();

            var existingStocks = await context.Stocks
                .Include(s => s.Financials)
                .ToListAsync();

            var incomingIds = incomingStocks.Where(s => s.Id > 0).Select(s => s.Id).ToHashSet();

            var stocksToRemove = existingStocks.Where(s => !incomingIds.Contains(s.Id)).ToList();
            if (stocksToRemove.Count > 0)
            {
                context.Stocks.RemoveRange(stocksToRemove);
            }

            foreach (var incomingStock in incomingStocks)
            {
                var existingStock = existingStocks.FirstOrDefault(s => s.Id == incomingStock.Id);
                if (existingStock == null)
                {
                    context.Stocks.Add(CloneStock(incomingStock));
                    continue;
                }

                MergeStock(existingStock, incomingStock);
            }

            await context.SaveChangesAsync();
        }

        private static Stock CloneStock(Stock stock)
        {
            var clone = new Stock
            {
                Name = stock.Name,
                Ticker = stock.Ticker,
                LastPrice = stock.LastPrice,
                LastUpdated = stock.LastUpdated,
                StockScore = stock.StockScore == null ? new StockScore() : stock.StockScore,
                Financials = new List<YearlyFinancials>()
            };

            foreach (var financial in stock.Financials ?? Enumerable.Empty<YearlyFinancials>())
            {
                clone.Financials.Add(CloneFinancial(financial));
            }

            return clone;
        }

        private static YearlyFinancials CloneFinancial(YearlyFinancials financial)
        {
            return new YearlyFinancials
            {
                Id = financial.Id, 
                Year = financial.Year,
                IsEstimate = financial.IsEstimate,
                Revenue = financial.Revenue,
                NmbrOfShares = financial.NmbrOfShares,
                StockPrice = financial.StockPrice,
                OperatingCashFlow = financial.OperatingCashFlow,
                Dividends = financial.Dividends,
                CapitalExpenditures = financial.CapitalExpenditures,
                KeyFiguresJson = financial.KeyFiguresJson,

                Earnings = financial.Earnings == null ? null : new Earning
                {
                    NetIncomeValue = financial.Earnings.NetIncomeValue,
                    EbitValue = financial.Earnings.EbitValue,
                    EbitdaValue = financial.Earnings.EbitdaValue,
                    EbitdaMargin = financial.Earnings.EbitdaMargin,
                    EbitMargin = financial.Earnings.EbitMargin,
                    NetIncomeMargin = financial.Earnings.NetIncomeMargin
                },
                EnterpriseVal = financial.EnterpriseVal == null ? null : new EnterpriseValue
                {
                    MarketValue = financial.EnterpriseVal.MarketValue,
                    LongTermDebt = financial.EnterpriseVal.LongTermDebt,
                    ShortTermDebt = financial.EnterpriseVal.ShortTermDebt,
                    CashAndEquivalents = financial.EnterpriseVal.CashAndEquivalents
                },
                KeyFiguresDict = financial.KeyFiguresDict == null ? new Dictionary<KeyFigureTypes, decimal>() : new Dictionary<KeyFigureTypes, decimal>(financial.KeyFiguresDict)
            };
        }

        private static void MergeStock(Stock existingStock, Stock incomingStock)
        {
            existingStock.Name = incomingStock.Name;
            existingStock.Ticker = incomingStock.Ticker;
            existingStock.LastPrice = incomingStock.LastPrice;
            existingStock.LastUpdated = incomingStock.LastUpdated;
            existingStock.StockScore = incomingStock.StockScore ?? new StockScore();

            var incomingFinancials = incomingStock.Financials ?? Enumerable.Empty<YearlyFinancials>().ToList();
            var incomingFinancialIds = incomingFinancials.Where(f => f.Id > 0).Select(f => f.Id).ToHashSet();

            for (var index = existingStock.Financials.Count - 1; index >= 0; index--)
            {
                var existingFinancial = existingStock.Financials[index];
                if (existingFinancial.Id > 0 && !incomingFinancialIds.Contains(existingFinancial.Id))
                {
                    existingStock.Financials.RemoveAt(index);
                }
            }

            foreach (var incomingFinancial in incomingFinancials)
            {
                var existingFinancial = existingStock.Financials.FirstOrDefault(f =>
                    (f.Id > 0 && f.Id == incomingFinancial.Id) ||
                    (f.Id == 0 && incomingFinancial.Id == 0 && f.Year == incomingFinancial.Year));

                if (existingFinancial == null)
                {
                    incomingFinancial.StockId = existingStock.Id;
                    existingStock.Financials.Add(CloneFinancial(incomingFinancial));
                }
                else
                {
                    existingFinancial.Year = incomingFinancial.Year;
                    existingFinancial.IsEstimate = incomingFinancial.IsEstimate;
                    existingFinancial.Revenue = incomingFinancial.Revenue;
                    existingFinancial.NmbrOfShares = incomingFinancial.NmbrOfShares;
                    existingFinancial.StockPrice = incomingFinancial.StockPrice;
                    existingFinancial.OperatingCashFlow = incomingFinancial.OperatingCashFlow;
                    existingFinancial.Dividends = incomingFinancial.Dividends;
                    existingFinancial.CapitalExpenditures = incomingFinancial.CapitalExpenditures;

                    existingFinancial.KeyFiguresDict = incomingFinancial.KeyFiguresDict ?? new Dictionary<KeyFigureTypes, decimal>();

                    existingFinancial.Earnings = incomingFinancial.Earnings == null ? null : new Earning
                    {
                        NetIncomeValue = incomingFinancial.Earnings.NetIncomeValue,
                        EbitValue = incomingFinancial.Earnings.EbitValue,
                        EbitdaValue = incomingFinancial.Earnings.EbitdaValue,
                        EbitdaMargin = incomingFinancial.Earnings.EbitdaMargin,
                        EbitMargin = incomingFinancial.Earnings.EbitMargin,
                        NetIncomeMargin = incomingFinancial.Earnings.NetIncomeMargin
                    };
                    existingFinancial.EnterpriseVal = incomingFinancial.EnterpriseVal == null ? null : new EnterpriseValue
                    {
                        MarketValue = incomingFinancial.EnterpriseVal.MarketValue,
                        LongTermDebt = incomingFinancial.EnterpriseVal.LongTermDebt,
                        ShortTermDebt = incomingFinancial.EnterpriseVal.ShortTermDebt,
                        CashAndEquivalents = incomingFinancial.EnterpriseVal.CashAndEquivalents
                    };
                }
            }
        }
    }
}
