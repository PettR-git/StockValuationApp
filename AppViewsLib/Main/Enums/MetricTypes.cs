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
    public enum MetricTypes
    {
        revenue,
        ebitda,
        ebit, 
        netIncome, 
        stockPrice,
        marketCapitalization,
        numberOfShares,
        shortTermDebt,
        longTermDebt,
        dividendsPaid, 
        cashAndCashEquivalents,
        operatingCashFlow,
        capitalExpenditure, 
        totalLiabilities, 
        totalAssets,
        depreciationAndAmortization
    }
}
