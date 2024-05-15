using StockValuationApp.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Entities.Stocks
{
    public class Metric
    {
        /// <summary>
        /// Metric and its value
        /// </summary>
        public (MetricType, double) MetricAndValue {  get; set; }
        
        /// <summary>
        /// Year of metrics data
        /// </summary>
        public int Year { get; set; }
    }
}
