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
    public class EnterpriseValue
    {
        public int MarketValue {  get; set; }
        public int NetDebt {  get; set; }
    }
}
