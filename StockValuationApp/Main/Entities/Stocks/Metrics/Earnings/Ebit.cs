using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Entities.Stocks.Metrics.Earnings
{
    public class Ebit : Earning
    {
        public override int EbitValue
        {
            get { return base.EbitValue; }
            set { base.EbitValue = value; }
        }
    }
}
