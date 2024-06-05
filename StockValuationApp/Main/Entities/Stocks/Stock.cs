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
    /// A stocks metric and financial data
    /// </summary>
    [Serializable]
    public class Stock
    {
        public Stock() {
            Financials = new List<YearlyFinancials>();
        }
        public string Name {  get; set; }
        public string Ticker { get; set; }

        /// <summary>
        /// Financials for a specific year
        /// </summary>
        public List<YearlyFinancials> Financials { get; set; }

        public override string ToString()
        {
            string outStr = string.Format("Stock: {0}, with ticker ${1}", Name, Ticker.ToUpper());
            return outStr;
        }
    }
}
