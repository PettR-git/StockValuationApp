using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Entities.Stocks.Metrics.Earnings
{
    public class Ebitda : Earning
    {
        public override int EbitdaValue
        {
            get { return base.EbitdaValue; }
            set { base.EbitdaValue = value; }
        }
    }
}
