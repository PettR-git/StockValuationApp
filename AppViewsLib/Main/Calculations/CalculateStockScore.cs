using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Main.Calculations
{
    public static class CalculateStockScore
    {
        public static decimal CalcRevenueGrowthScore(double[] revenue)
        {
            double prevRev = 0, currRev = 0, addedGrowth = 0, cagr = 0;
            double[] intervals = {0, 8, 12, 17, 22, 100};
            decimal result = 0;

            foreach(var rev in revenue)
            {
                currRev = rev;
                addedGrowth = 100 * ((currRev - prevRev) / prevRev);
            }

            cagr = (double)(addedGrowth / revenue.Length);

            if (cagr < intervals[1])
            {
                result = GradientScore(cagr, 1, intervals[0], intervals[1]);
            }
            else if (cagr <= intervals[2])
            {
                result = GradientScore(cagr, 2, intervals[1], intervals[2]);
            }
            else if (cagr <= intervals[3])
            {
                result = GradientScore(cagr, 3, intervals[2], intervals[3]);
            }
            else if (cagr <= intervals[4])
            {
                result = GradientScore(cagr, 4, intervals[3], intervals[4]);
            }
            else if (cagr <= intervals[5])
            {
                result = GradientScore(cagr, 5, intervals[4], intervals[5]);
            }

            return result;
        }

        private static decimal GradientScore(double gradedVal, double startScore, double startVal, double endVal)
        {
            return (decimal)Math.Round(startScore + (gradedVal - startVal) / (endVal - startVal), 1);
        }

        public static decimal CalcEarningsGrowthScore(double[] netIncome)
        {
            double prevInc = 0, currInc = 0, addedGrowth = 0, cagr = 0;
            double[] intervals = { 0, 7, 12, 17, 24, 100 };
            decimal result = 0;

            foreach (var netInc in netIncome)
            {
                currInc = netInc;
                addedGrowth = 100 * ((currInc - prevInc) / prevInc);
            }

            cagr = (double)(addedGrowth / netIncome.Length);

            if (cagr < intervals[1])
            {
                result = GradientScore(cagr, 1, intervals[0], intervals[1]);
            }
            else if (cagr <= intervals[2])
            {
                result = GradientScore(cagr, 2, intervals[1], intervals[2]);
            }
            else if (cagr <= intervals[3])
            {
                result = GradientScore(cagr, 3, intervals[2], intervals[3]);
            }
            else if (cagr <= intervals[4])
            {
                result = GradientScore(cagr, 4, intervals[3], intervals[4]);
            }
            else if (cagr <= intervals[5])
            {
                result = GradientScore(cagr, 5, intervals[4], intervals[5]);
            }

            return result;
        }

        public static decimal CalcEvEbitFfcScore(decimal[] evEbit, decimal[] evFcf)
        {
            return default;
        }

        public static decimal CalcRoeRoicScore(decimal[] roe, decimal[] roic)
        {
            return default;
        }
    }
}
