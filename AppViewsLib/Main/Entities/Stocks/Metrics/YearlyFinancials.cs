using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockValuationApp.Entities.Enums;
using StockValuationApp.Entities.Stocks.Metrics.Earnings;

namespace StockValuationApp.Entities.Stocks.Metrics
{
    /// <summary>
    /// Holds all data for metrics in a specific year.
    /// </summary>
    [Serializable]
    public class YearlyFinancials
    {
        //Metric and its value
        public Dictionary <KeyFigureTypes, double> KeyFiguresDict { get; set; }

        /// Properties for year and components of a metric
        public int Year { get; set; }
        public double Revenue { get; set; } = 0;
        public double NmbrOfShares { get; set; } = 0;
        public double StockPrice {  get; set; } = 0;
        public Earning Earnings { get; set; }
        public EnterpriseValue EnterpriseVal { get; set; }

        public override string ToString()
        {
            string outStr = string.Empty;
            string metricStr = string.Empty;

            if (KeyFiguresDict != null)
            {
                foreach (var kvp in KeyFiguresDict)
                {
                    switch (kvp.Key)
                    {
                        case KeyFigureTypes.EvEbitda:
                            metricStr = "EV/EBITDA";
                            break;
                        case KeyFigureTypes.EvEbit:
                            metricStr = "EV/EBIT";
                            break;
                        case KeyFigureTypes.PriceToEarnings:
                            metricStr = "P/E";
                            break;
                        case KeyFigureTypes.NetDebtToEbitda:
                            metricStr = "Net Debt/EBITDA";
                            break;
                    }

                    if (Year > DateTime.Now.Year)
                        outStr += "Estimation | ";

                    outStr += string.Format("{0}: {1:F2} | Year {2}\n", metricStr, kvp.Value, Year);
                }
            }
            else
            {
                outStr = $"Insufficient data for key figures | Year {Year}";
            }
            return outStr.TrimEnd();
        }
    }
}
