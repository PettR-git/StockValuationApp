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
        public static double CalcRoe(double netIncome, double totAssets, double totLiabilities)
        {
            double result = 100*(netIncome/(totAssets-totLiabilities));

            return result;
        }

        public static double CalcRoic(double netIncome, double dividends, double longTermDebt, double shortTermDebt, double totAssets, double totLiabilities)
        {
            double result = 100*((netIncome - dividends)/(longTermDebt + shortTermDebt + totAssets - totLiabilities));

            return result;
        }

        public static double CalcEvFreeCashflow((double marketVal, double shortTermDebt, double longTermDebt, double cash) ev, double operCF, double capExp)
        {
            double result = (ev.marketVal + ev.shortTermDebt + ev.longTermDebt - ev.cash) / (operCF - capExp);

            return result;
        }

        public static double CalcEvEarnings((double marketVal, double shortTermDebt, double longTermDebt, double cash)ev, double earnings)
        {
            double result = (ev.marketVal + ev.shortTermDebt + ev.longTermDebt - ev.cash)/earnings;

            return result;
        }

        public static double CalcPriceToEarnings((double netIncome, double nmbrOfShares)eps, double price)
        {
            double result = price / (eps.netIncome / eps.nmbrOfShares);

            return result;
        }

        public static double CalcNetDebtToEbitda((double shortTermDebt, double longTermDebt, double cash)netDebt, double ebitda)
        {
            double result = (netDebt.longTermDebt + netDebt.shortTermDebt - netDebt.cash) / ebitda;

            return result;
        }
    }
}
