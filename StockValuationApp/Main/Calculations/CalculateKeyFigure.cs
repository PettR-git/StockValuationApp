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
    public static class CalculateKeyFigure
    {
        public static double CalcEvEarnings((int marketVal, int shortTermDebt, int longTermDebt, int cash)ev, int earnings)
        {
            int netDebtVal = ev.shortTermDebt + ev.longTermDebt - ev.cash;
            int evVal = ev.marketVal + netDebtVal;
            double result = (double)evVal / earnings;

            return result;
        }

        public static double CalcPriceToEarnings((int netIncome, int nmbrOfShares)eps, int price)
        {
            double epsVal = (double)eps.netIncome / eps.nmbrOfShares;
            double result = (double)price / epsVal;

            return result;
        }

        public static double CalcNetDebtToEbitda((int shortTermDebt, int longTermDebt, int cash)netDebt, int ebitda)
        {
            double netDebtVal = netDebt.longTermDebt + netDebt.shortTermDebt - netDebt.cash;
            double result = (double)netDebtVal / ebitda;

            return result;
        }
    }
}
