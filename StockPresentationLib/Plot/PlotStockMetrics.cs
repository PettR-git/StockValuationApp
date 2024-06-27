using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using StockValuationApp.Entities.Stocks.Metrics;
using StockValuationApp.Main.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPresentationLib.Plot
{
    public class PlotStockMetrics
    {
        private List<Tick> xAxesYears;
        private List<YearlyFinancials> yearlyFinancials;
        private WpfPlot finPlot;
        private Scatter scatterEbitdaGrowth;
        private Scatter scatterEbitGrowth;
        private Scatter scatterNetIncGrowth;
        private Scatter scatterRevGrowth;
        public PlotStockMetrics(WpfPlot finPlot, List<YearlyFinancials> yearlyFinancials) 
        { 
            this.finPlot = finPlot;
            this.yearlyFinancials = yearlyFinancials;
        }

        public void PlotRevenueAndEarnings()
        {
            //Bar graph and Scatter line growth
            ScottPlot.Palettes.Category10 palette = new ScottPlot.Palettes.Category10();
            List<Bar> bars = new List<Bar>();
            xAxesYears = new List<Tick>();

            for (int i = 0, indexPos = 0, indexBar = 0, posTick = 0; i < yearlyFinancials.Count(); i++, indexPos += 2, indexBar++, posTick += 4)
            {
                try
                {
                    if (i < yearlyFinancials.Count())
                    {
                        double revenue = yearlyFinancials.ElementAt(i).Revenue;
                        double ebit = 0, ebitda = 0, netIncome = 0;

                        if (yearlyFinancials.ElementAt(i).Earnings != null)
                        {
                            if (yearlyFinancials.ElementAt(i).Earnings.EbitdaValue != 0)
                            {
                                ebitda = yearlyFinancials.ElementAt(i).Earnings.EbitdaValue;
                            }
                            else
                            {
                                ebit = yearlyFinancials.ElementAt(i).Earnings.EbitValue;
                            }

                            netIncome = yearlyFinancials.ElementAt(i).Earnings.NetIncomeValue;
                        }

                        xAxesYears.Add(new Tick(posTick, yearlyFinancials.ElementAt(i).Year.ToString()));
                        //bars.Add(revenue);
                        //bars.Add(ebitda);
                        //bars.Add(netIncome);
                        bars.Add(new Bar { Position = indexPos, Value = revenue, FillColor = palette.GetColor(0) });
                        bars.Add(new Bar { Position = ++indexPos, Value = ebitda, FillColor = palette.GetColor(1) });
                        bars.Add(new Bar { Position = ++indexPos, Value = netIncome, FillColor = palette.GetColor(2) });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            finPlot.Plot.Add.Bars(bars);

            //Configure extras
            finPlot.Plot.Legend.IsVisible = true;
            finPlot.Plot.Legend.Alignment = Alignment.UpperLeft;
            finPlot.Plot.Legend.ManualItems.Add(new LegendItem { LabelText = MetricTypes.revenue.ToString(), FillColor = palette.GetColor(0) });
            finPlot.Plot.Legend.ManualItems.Add(new LegendItem { LabelText = MetricTypes.ebitda.ToString(), FillColor = palette.GetColor(1) });
            finPlot.Plot.DataBackground.Color = Color.FromHex("#0b3049");

            finPlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(xAxesYears.ToArray());
            finPlot.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            finPlot.Plot.Axes.Margins(bottom: 0, left: 0.7, right: 0.7);

            finPlot.Refresh();
        }

        public void PlotEbitdaGrowth(bool showPlot)
        {
            if (showPlot)
            {
                List<double> ebitdaGrowth = new List<double>() { 0.0 };
                List<double> positions = new List<double>();

                if (yearlyFinancials != null && yearlyFinancials.Where(x => x.Earnings != null).Sum(x => x.Earnings.EbitdaValue) > 0)
                {
                    for (int i = 0; i < yearlyFinancials.Count() - 1; i++)
                    {
                        if (yearlyFinancials.ElementAt(i).Earnings != null)
                        {
                            double ebitdaPrev = yearlyFinancials.ElementAt(i).Earnings.EbitdaValue;
                            if (ebitdaPrev != 0)
                            {
                                double ebitdaCurr = yearlyFinancials.ElementAt(i + 1).Earnings.EbitdaValue;
                                ebitdaGrowth.Add(100 * ((double)(ebitdaCurr - ebitdaPrev) / ebitdaPrev));
                            }
                            else
                            {
                                ebitdaGrowth.Add(0.0);
                            }

                            positions.Add(xAxesYears[i].Position);
                        }
                    }
                    positions.Add(xAxesYears.Last().Position);

                    scatterEbitdaGrowth = finPlot.Plot.Add.Scatter(positions, ebitdaGrowth);
                    scatterEbitdaGrowth.Axes.YAxis = finPlot.Plot.Axes.Right;
                }
            }
            else
            {
                finPlot.Plot.Remove(scatterEbitdaGrowth);
            }

            finPlot.Refresh();
        }

        public void PlotRevenueGrowth(bool showPlot)
        {
            if (showPlot)
            {
                List<double> revenueGrowth = new List<double>() { 0.0 };
                List<double> positions = new List<double>();

                if (yearlyFinancials != null && yearlyFinancials.Sum(x => x.Revenue) > 0)
                {
                    for (int i = 0; i < yearlyFinancials.Count() - 1; i++)
                    {
                        double revPrev = yearlyFinancials.ElementAt(i).Earnings.EbitValue;
                        if (revPrev != 0)
                        {
                            double revCurr = yearlyFinancials.ElementAt(i + 1).Earnings.EbitValue;
                            revenueGrowth.Add(100 * ((double)(revCurr - revPrev) / revPrev));
                        }
                        else
                        {
                            revenueGrowth.Add(0.0);
                        }

                        positions.Add(xAxesYears[i].Position);
                    }
                }
                positions.Add(xAxesYears.Last().Position);

                scatterRevGrowth = finPlot.Plot.Add.Scatter(positions, revenueGrowth);
                scatterRevGrowth.Axes.YAxis = finPlot.Plot.Axes.Right;
            }
            else
            { 
                finPlot.Plot.Remove(scatterRevGrowth);
            }

            finPlot.Refresh();
        }

        public void PlotEbitGrowth(bool showPlot)
        {
            if (showPlot)
            {
                List<double> ebitGrowth = new List<double>() { 0.0 };
                List<double> positions = new List<double>();

                if (yearlyFinancials != null && yearlyFinancials.Where(x => x.Earnings != null).Sum(x => x.Earnings.EbitValue) > 0)
                {
                    for (int i = 0; i < yearlyFinancials.Count() - 1; i++)
                    {
                        double ebitPrev = yearlyFinancials.ElementAt(i).Earnings.EbitValue;
                        if (ebitPrev != 0)
                        {
                            double ebitCurr = yearlyFinancials.ElementAt(i + 1).Earnings.EbitValue;
                            ebitGrowth.Add(100 * ((double)(ebitCurr - ebitPrev) / ebitPrev));
                        }
                        else
                        {
                            ebitGrowth.Add(0.0);
                        }

                        positions.Add(xAxesYears[i].Position);
                    }
                }
                positions.Add(xAxesYears.Last().Position);

                scatterEbitGrowth = finPlot.Plot.Add.Scatter(positions, ebitGrowth);
                scatterEbitGrowth.Axes.YAxis = finPlot.Plot.Axes.Right;
            }
            else
            {
                finPlot.Plot.Remove(scatterEbitGrowth);
            }

            finPlot.Refresh();
        }


        public void PlotNetIncomeGrowth(bool showPlot)
        {
            if (showPlot)
            {
                List<double> nIncomeGrowth = new List<double>() { 0.0 };
                List<double> positions = new List<double>();

                if (yearlyFinancials != null && yearlyFinancials.Where(x => x.Earnings != null).Sum(x => x.Earnings.NetIncomeValue) > 0)
                {
                    for (int i = 0; i < yearlyFinancials.Count() - 1; i++)
                    {
                        double nIncPrev = yearlyFinancials.ElementAt(i).Earnings.NetIncomeValue;
                        if (nIncPrev != 0)
                        {
                            double incCurr = yearlyFinancials.ElementAt(i + 1).Earnings.NetIncomeValue;
                            nIncomeGrowth.Add(100 * ((double)(incCurr - nIncPrev) / nIncPrev));
                        }
                        else
                        {
                            nIncomeGrowth.Add(0.0);
                        }

                        positions.Add(xAxesYears[i].Position);
                    }
                    positions.Add(xAxesYears.Last().Position);

                    scatterNetIncGrowth = finPlot.Plot.Add.Scatter(positions, nIncomeGrowth);
                    scatterNetIncGrowth.Axes.YAxis = finPlot.Plot.Axes.Right;
                }
            }
            else
            {
                finPlot.Plot.Remove(scatterNetIncGrowth);
            }

            finPlot.Refresh();
        }

        public void RenderPlot()
        {
            finPlot.Refresh();
        }
    }
}
