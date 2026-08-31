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

        //Load all stocks with their yearly financials
        public async Task<List<Stock>> LoadAllAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();

            return await context.Stocks
                .AsNoTracking()
                .Include(s => s.YearlyFinancials)
                .ToListAsync();
        }

        //Just load stock overview data, not the full details
        public async Task<List<Stock>> LoadStockOverviewsAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();

            return await context.Stocks
                .AsNoTracking()
                .Select(s => new Stock
                {
                    Id = s.Id,
                    Name = s.Name,
                    Ticker = s.Ticker,
                    LastPrice = s.LastPrice,
                    StockScore = s.StockScore
                })
                .ToListAsync();
        }

        // Load full stock details including yearly financials
        public async Task<Stock?> GetStockDetailsAsync(int stockId)
        {
            await using var context = await _factory.CreateDbContextAsync();

            return await context.Stocks
                .AsNoTracking()
                .Include(s => s.YearlyFinancials)
                .FirstOrDefaultAsync(s => s.Id == stockId);
        }

        public async Task SaveAllAsync(IEnumerable<Stock> stocks)
        {
            var incomingStocks = stocks?.ToList() ?? new List<Stock>();

            await using var context = await _factory.CreateDbContextAsync();

            var incomingIds = incomingStocks.Where(s => s.Id > 0).Select(s => s.Id).ToList();
            await context.Stocks
                .Where(s => !incomingIds.Contains(s.Id))
                .ExecuteDeleteAsync();

            foreach (var stock in incomingStocks)
            {
                if (stock.Id == 0)
                {
                    //New stock, add it to the context
                    context.Stocks.Add(stock);
                }
                else
                {
                    context.Stocks.Update(stock);

                    //Delete any yearly financials that are no longer present in the incoming stock
                    var financialIds = stock.YearlyFinancials.Select(f => f.Id).ToList();
                    await context.YearlyFinancials
                        .Where(f => f.StockId == stock.Id && !financialIds.Contains(f.Id))
                        .ExecuteDeleteAsync();
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
