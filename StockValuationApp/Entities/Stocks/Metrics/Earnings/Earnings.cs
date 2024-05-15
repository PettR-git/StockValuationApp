using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Entities.Stocks.Metrics.Earnings
{
    public abstract class Earnings
    {
        public virtual int EbitValue { get; set; } = -1;
        public virtual int EbitdaValue { get; set; } = -1;
    }
}
