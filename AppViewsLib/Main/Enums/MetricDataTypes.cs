using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Main.Enums
{
    /// <summary>
    /// For specific API-calls
    /// </summary>
    public enum MetricDataTypes
    {
        totalRevenue,
        ebitda,
        ebit, 
        netIncome, 
        closePrice,
        shares_outstanding_basic,
        shortTermDebt,
        longTermDebt,
        dividendPayout, 
        cashAndCashEquivalentsAtCarryingValue,
        operatingCashFlow,
        capitalExpenditures, 
        totalLiabilities, 
        totalAssets,
        depreciationAndAmortization,
        date
    }
}
