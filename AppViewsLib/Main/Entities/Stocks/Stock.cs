using StockLib.Main.Entities.Stocks;
using StockValuationApp.Entities.Stocks.Metrics;
using StockValuationApp.Entities.Stocks.Metrics.Earnings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Entities.Stocks
{
    /// <summary>
    /// A stock's metric and financial data.
    /// </summary>
    [Serializable]
    public class Stock
    {
        public EventHandler<MetricEventArgs> MetricsGiven;

        public Stock()
        {
            Financials = new List<YearlyFinancials>();
            StockScore = new StockScore();
            LastUpdated = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Ticker { get; set; } = string.Empty;
        public decimal LastPrice { get; set; }
        public DateTime LastUpdated { get; set; }

        // Financials for a specific year
        public List<YearlyFinancials> Financials { get; set; }

        // Score
        public StockScore? StockScore { get; set; }

        public override string ToString()
        {
            return $"Stock: {Name}, ${Ticker.ToUpperInvariant()}";
        }
    }
}
