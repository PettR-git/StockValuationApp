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
        public static decimal CalcRoe(double netIncome, double totAssets, double totLiabilities)
        {
            double divisor = totAssets - totLiabilities;

            if(divisor == 0)
            {
                divisor = Math.Pow(1, -8);
            }

            decimal result = (decimal)Math.Round(100*(netIncome/(divisor)),1);

            return result;
        }

        public static decimal CalcRoic(double netIncome, double dividends, double longTermDebt, double shortTermDebt, double totAssets, double totLiabilities)
        {
            double divisor = longTermDebt + shortTermDebt + totAssets - totLiabilities;

            if(divisor == 0)
            {
                divisor = Math.Pow(1, -8);
            }

            decimal result = (decimal)Math.Round(100*((netIncome - dividends)/(divisor)),1);

            return result;
        }

        public static decimal CalcEvFreeCashflow((double marketVal, double shortTermDebt, double longTermDebt, double cash) ev, double operCF, double capExp)
        {
            double divisor = operCF - capExp;

            if (divisor == 0)
            {
                divisor = Math.Pow(1, -8);
            }

            decimal result = (decimal)Math.Round(((ev.marketVal + ev.shortTermDebt + ev.longTermDebt - ev.cash) / (divisor)), 1);

            return result;
        }

        public static decimal CalcEvEarnings((double marketVal, double shortTermDebt, double longTermDebt, double cash)ev, double earnings)
        {
            if (earnings == 0)
            {
                earnings = Math.Pow(1, -8);
            }

            decimal result = (decimal)Math.Round(((ev.marketVal + ev.shortTermDebt + ev.longTermDebt - ev.cash) / earnings), 1);

            return result;
        }

        public static decimal CalcPriceToEarnings((double netIncome, double nmbrOfShares)eps, double price)
        {
            decimal result = (decimal)Math.Round((price / (eps.netIncome / eps.nmbrOfShares)), 1);

            return result;
        }

        public static decimal CalcNetDebtToEbitda((double shortTermDebt, double longTermDebt, double cash)netDebt, double ebitda)
        { 
            if (ebitda == 0)
            {
                ebitda = Math.Pow(1, -8);
            }

            decimal result = (decimal)Math.Round((netDebt.longTermDebt + netDebt.shortTermDebt - netDebt.cash) / ebitda, 1);

            return result;
        }
    }
}
