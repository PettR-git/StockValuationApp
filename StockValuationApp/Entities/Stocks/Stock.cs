using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Entities.Stocks
{
    public class Stock
    {
        public string Name {  get; set; }
        public string Ticker { get; set; }
        public int Revenue { get; set; } = 0;
        public int NmbrOfShares { get; set; } = 0;
        public List<Metric> Metrics { get; set; }

    }
}
