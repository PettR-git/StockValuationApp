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
        public static double CalcEvEarnings((double marketVal, double shortTermDebt, double longTermDebt, double cash)ev, double earnings)
        {
            double netDebtVal = ev.shortTermDebt + ev.longTermDebt - ev.cash;
            double evVal = ev.marketVal + netDebtVal;
            double result = evVal/earnings;

            return result;
        }

        public static double CalcPriceToEarnings((double netIncome, double nmbrOfShares)eps, double price)
        {
            double epsVal = eps.netIncome / eps.nmbrOfShares;
            double result = price / epsVal;

            return result;
        }

        public static double CalcNetDebtToEbitda((double shortTermDebt, double longTermDebt, double cash)netDebt, double ebitda)
        {
            double netDebtVal = netDebt.longTermDebt + netDebt.shortTermDebt - netDebt.cash;
            double result = netDebtVal / ebitda;

            return result;
        }
    }
}
