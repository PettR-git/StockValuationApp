﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Entities.Stocks.Metrics.Earnings
{
    [Serializable]
    public class Ebitda : Earning
    {
        public override double EbitdaValue
        {
            get { return base.EbitdaValue; }
            set { base.EbitdaValue = value; }
        }
    }
}
