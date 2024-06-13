using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Entities.Stocks.Metrics
{
    /// <summary>
    /// Enterprise Value and its properties
    /// </summary>
    [Serializable]
    public class EnterpriseValue
    {
        public double MarketValue {  get; set; }
        public double LongTermDebt {  get; set; }
        public double ShortTermDebt { get; set; }
        public double CashAndEquivalents { get; set; }
    }
}
