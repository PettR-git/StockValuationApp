using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Entities.Calculations
{
    /// <summary>
    /// Provide different methods for calculating valuation metrics.
    /// Be aware, static class
    /// </summary>
    public static class CalculateValuationMetric
    {
        public static double CalcEvEarnings((int marketVal, int netDebt)ev, int earnings)
        {
            int evVal = ev.marketVal + ev.netDebt;
            double result = (double)evVal / earnings;

            return result;
        }

        public static double CalcPriceToEarnings((int netIncome, int nmbrOfShares)eps, int price)
        {
            double epsVal = (double)eps.netIncome / eps.nmbrOfShares;
            double result = (double)price / epsVal;

            return result;
        }

        public static double CalcNetDebtToEbitda(int netDebt, int ebitda)
        {
            double result = (double)netDebt / ebitda;

            return result;
        }
    }
}
