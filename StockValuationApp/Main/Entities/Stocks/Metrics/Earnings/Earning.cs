using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Entities.Stocks.Metrics.Earnings
{
    [Serializable]
    public class Earning
    {
        public virtual double NetIncomeValue { get; set; } = 0;
        public virtual double EbitValue { get; set; } = 0;
        public virtual double EbitdaValue { get; set; } = 0; 
    }
}
