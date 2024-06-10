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
        public int Revenue {  get; set; }
        public int Ebitda {  get; set; }
        public int Ebit {  get; set; }
        public int NetIncome {  get; set; }
        public int Price {  get; set; }
        public int MarketValue {  get; set; }   
        public int NumberOfShares {  get; set; }
        public int ShortTermDebt { get; set; }
        public int LongTermDebt { get; set; }
        public double Dividends {  get; set; }
        public int CashAndEquivalents {  get; set; }
        public int OperationalCashflow {  get; set; }
        public int CapitalExpenditures {  get; set; }
        public int TotalLiabilities {  get; set; }
        public int TotalAssets {  get; set; }
    }
}
