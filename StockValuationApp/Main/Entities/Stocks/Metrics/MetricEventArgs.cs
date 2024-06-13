using StockValuationApp.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Entities.Stocks
{
    /// <summary>
    /// All event args for metric data for a specific stock and year
    /// </summary>
    public class MetricEventArgs
    {
        public Stock Stock { get; set; }
        public int Year { get; set; }    
        public double Revenue {  get; set; }
        public double Ebitda {  get; set; }
        public double Ebit {  get; set; }
        public double NetIncome {  get; set; }
        public double Price {  get; set; }
        public double MarketValue {  get; set; }   
        public double NumberOfShares {  get; set; }
        public double ShortTermDebt { get; set; }
        public double LongTermDebt { get; set; }
        public double Dividends {  get; set; }
        public double CashAndEquivalents {  get; set; }
        public double OperationalCashflow {  get; set; }
        public double CapitalExpenditures {  get; set; }
        public double TotalLiabilities {  get; set; }
        public double TotalAssets {  get; set; }
    }
}
