using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Entities.Stocks.Metrics.Earnings
{
    public class Earning
    {
        public virtual int NetIncomeValue { get; set; } = 0;
        public virtual int EbitValue { get; set; } = 0;
        public virtual int EbitdaValue { get; set; } = 0; 
    }
}
