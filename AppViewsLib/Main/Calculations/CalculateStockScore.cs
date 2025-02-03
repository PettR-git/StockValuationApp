using StockValuationApp.Entities.Stocks.Metrics.Earnings;
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
        public static double CalcRevenueGrowthScore(double[] revenue)
        {
            double[] intervals = {0, 7, 14, 18, 25, 100};
            double result = 0;

            double cagr = CalcAverageGrowth(revenue);
            result = AscendingGradientScore(intervals, cagr);

            return result;
        }

        private static double CalcAscGradientScore(double gradedVal, double startScore, double startVal, double endVal)
        {
            return Math.Round(startScore + Math.Abs((gradedVal - startVal) / (endVal - startVal)), 1);
        }

        private static double CalcDescGradientScore(double gradedVal, double startScore, double startVal, double endVal)
        {
            return Math.Round(startScore + Math.Abs((endVal - gradedVal) / (endVal - startVal)), 1);
        }

        //Scoring 0 to 5, higher values higher score
        private static double AscendingGradientScore(double[] intervals, double compVal)
        {
            double result = 0;

            if (compVal <= intervals[1])
            {
                if (compVal <= intervals[0])
                {
                    result = 0;
                }
                else
                {
                    result = CalcAscGradientScore(compVal, 0, intervals[0], intervals[1]);
                }
            }
            else if (compVal <= intervals[2])
            {
                result = CalcAscGradientScore(compVal, 1, intervals[1], intervals[2]);
            }
            else if (compVal <= intervals[3])
            {
                result = CalcAscGradientScore(compVal, 2, intervals[2], intervals[3]);
            }
            else if (compVal <= intervals[4])
            {
                result = CalcAscGradientScore(compVal, 3, intervals[3], intervals[4]);
            }
            else
            {
                if(compVal >= intervals[5])
                {
                    result = 5;
                }
                else
                {
                    result = CalcAscGradientScore(compVal, 4, intervals[4], intervals[5]);
                }
            }

            return result;
        }

        //Scoring 0 to 5, lower values higher score
        private static double DescendingGradientScore(double[] intervals, double compVal)
        {
            double result = 0;

            if (compVal <= intervals[1])
            {
                if(compVal < intervals[0])
                {
                    result = 0.0001;
                }
                else
                {
                    result = CalcDescGradientScore(compVal, 4, intervals[0], intervals[1]);
                }
            }
            else if (compVal <= intervals[2])
            {
                result = CalcDescGradientScore(compVal, 3, intervals[1], intervals[2]);
            }
            else if (compVal <= intervals[3])
            {
                result = CalcDescGradientScore(compVal, 2, intervals[2], intervals[3]);
            }
            else if (compVal <= intervals[4])
            {
                result = CalcDescGradientScore(compVal, 1, intervals[3], intervals[4]);
            }
            else
            {
                if (compVal >= intervals[5])
                {
                    result = 0;
                }
                else
                {
                    result = CalcDescGradientScore(compVal, 0, intervals[4], intervals[5]);
                }
            }

            return result;
        }

        private static double CalcAverageGrowth(double[] growthVals)
        {
            double averageGrowth = 0;
            double initialVal = growthVals[0];
            double lastVal = growthVals[growthVals.Length - 1];

            if(initialVal == 0 || lastVal == 0)
            {
                averageGrowth = 0;
            }
            else if (initialVal < 0)
            {
                averageGrowth = 100 * ((lastVal - initialVal) / -initialVal);
            }
            else
            {
                averageGrowth = 100 * ((lastVal - initialVal) / initialVal);
            }

            return averageGrowth;
        }

        public static double CalcEpsGrowthScore(double[] netIncome, double[] numberOfShares)
        {
            double[] intervals = { 0, 7, 12, 17, 24, 100 };
            double[] eps = new double[netIncome.Length];
            double result = 0;

            if(netIncome.Length != numberOfShares.Length)
            {
                return 0;
            }
            else
            {
                for(int i = 0; i < netIncome.Length; i++)
                {
                    eps[i] = netIncome[i]/numberOfShares[i];
                }
            }

            double cagr = CalcAverageGrowth(eps);
            result = AscendingGradientScore(intervals, cagr);

            return result;
        }

        public static double CalcEvFcfScore(double evFcf)
        {
            double evFcfScore = 0;
            double[] interEvFcf = {0, 12, 16, 24, 30, 50 };

            evFcfScore = DescendingGradientScore(interEvFcf, evFcf);

            return evFcfScore;
        }

        public static double CalcRoeRoicScore(double[] roe, double[] roic)
        {
            double[] interRoeGrowth = { -30, -15, 0, 5, 10, 20 };
            double[] interRoicGrowth = { -30, -15, 0, 5, 10, 20 };
            double[] growthIntervals = { -30, -15, 0, 5, 10, 20 };
            double[] roeIntervals = { 0, 5, 10, 15, 20, 40 };
            double[] roicIntervals = { 0, 5, 10, 15, 20, 40 };

            double averReturnGrowth = 0, averRoeGrowth = 0, averRoicGrowth;
            double growthResult = 0, roeScore = 0, roicScore = 0, totScore;

            averRoeGrowth = CalcAverageGrowth(roe);
            averRoicGrowth = CalcAverageGrowth(roic);
            averReturnGrowth = (averRoeGrowth + averRoicGrowth)/2;
            growthResult = AscendingGradientScore(growthIntervals, averReturnGrowth);

            roeScore = AscendingGradientScore(roeIntervals, roe[roe.Length-1]);
            roicScore = AscendingGradientScore(roicIntervals, roic[roic.Length-1]);

            totScore = (((roeScore + roicScore) / 2) + growthResult)/ 2;

            return totScore;
        }

        public static double CalcEvEbitScore(double evEbit)
        {
            double evEbitScore = 0;
            double[] interEvEbit = { 0, 8, 15, 21, 26, 50 };

            evEbitScore = DescendingGradientScore(interEvEbit, evEbit);

            return evEbitScore;
        }
    }
}
