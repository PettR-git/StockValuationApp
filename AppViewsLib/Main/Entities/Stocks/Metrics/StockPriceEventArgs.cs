using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Main.Entities.Stocks.Metrics
{

    public readonly record struct StockBar(
        string Symbol,
        DateTimeOffset Timestamp,
        decimal Open,
        decimal High,
        decimal Low,
        decimal Close,
        long Volume
    );

    public class WeeklyStockPricesEventArgs : EventArgs
    {
        public string Symbol { get; }
        public DateTimeOffset StartDate { get; }
        public DateTimeOffset EndDate { get; }

        public IReadOnlyList<StockBar> Bars { get; }

        public WeeklyStockPricesEventArgs(
            string symbol,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            IReadOnlyList<StockBar> bars)
        {
            Symbol = symbol;
            StartDate = startDate;
            EndDate = endDate;
            Bars = bars ?? Array.Empty<StockBar>();
        }
    }
}
