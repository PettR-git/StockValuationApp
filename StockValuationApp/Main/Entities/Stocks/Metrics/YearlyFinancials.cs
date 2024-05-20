using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockValuationApp.Entities.Enums;
using StockValuationApp.Entities.Stocks.Metrics.Earnings;

namespace StockValuationApp.Entities.Stocks.Metrics
{
    public class YearlyFinancials
    {
        /// <summary>
        /// Metric and its value
        /// </summary>
        public Dictionary <MetricType, double> MetricDict { get; set; }

        public string Year { get; set; } = DateTime.Now.Year.ToString();
        public int Revenue { get; set; } = 0;
        public int NmbrOfShares { get; set; } = 0;
        public Earning Earnings { get; set; }
        public EnterpriseValue EnterpriseVal { get; set; }

        public override string ToString()
        {
            string outStr = string.Empty;
            string metricStr = string.Empty;

            foreach (var kvp in MetricDict)
            {
                switch (kvp.Key)
                {
                    case MetricType.EvEbitda:
                        metricStr = "EV/EBITDA";
                        break;
                    case MetricType.EvEbit:
                        metricStr = "EV/EBIT";
                        break;
                    case MetricType.PriceToEarnings:
                        metricStr = "P/E";
                        break;
                }
                outStr += string.Format("{0}: {1} | Year {2}\n", metricStr, kvp.Value, Year);
            }
            return outStr;
        }
    }
}
