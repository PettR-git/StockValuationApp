using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Entities.Stocks.Metrics.Earnings
{
    public class NetIncome : Earning
    {
        public override int NetIncomeValue
        {
            get { return base.NetIncomeValue; }
            set { base.NetIncomeValue = value; }
        }
    }
}
