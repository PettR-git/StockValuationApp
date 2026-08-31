using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using StockValuationApp.Entities.Stocks;
using StockValuationApp.Entities.Stocks.Metrics;
using StockValuationApp.Main.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPresentationLib.Plot
{
    public class PlotEarnings
    {
        private List<Tick> xAxesYears;
        private List<YearlyFinancials> yearlyFinancials;
        private string stockStr;
        private WpfPlot finPlot;
        private Bar[] barArr;
        private Scatter scatterEbitdaGrowth;
        private Scatter scatterEbitGrowth;
        private Scatter scatterNetIncGrowth;
        private Scatter scatterRevGrowth;
        private ScottPlot.Palettes.Category10 palette;
        private const int scatterLineWidth = 4;
        private const int scatterMarkerSize = 10;
        private const double scatterLineOpacity = 0.55;
        public PlotEarnings(WpfPlot finPlot, string stockToStr, List<YearlyFinancials> yearlyFinancials)
        {
            UpdatePlotAndStock(finPlot, stockToStr);
            this.yearlyFinancials = yearlyFinancials;
            palette = new ScottPlot.Palettes.Category10();
        }

        public void UpdatePlotAndStock(WpfPlot finPlot, string stockStr)
        {
            this.finPlot = finPlot;
            this.stockStr = stockStr;
        }

        public string StockStr {  get { return stockStr; } }

        public void PlotRevenueAndEarnings()
        {
            // Clear old data to prevent overlapping when called multiple times
            finPlot.Plot.Clear();

            List<Bar> bars = new List<Bar>();
            xAxesYears = new List<Tick>();

            double currentX = 0; // Tracks precise horizontal placement
            double barSpacing = 0.1; // Small gap between bars within the same year
            double yearSpacing = 1.5; // Gap between year groups

            for (int i = 0; i < yearlyFinancials.Count; i++)
            {
                try
                {
                    var financial = yearlyFinancials[i];
                    double ebit = 0, ebitda = 0, netIncome = 0;

                    ScottPlot.Color revColor = palette.GetColor(0);
                    ScottPlot.Color ebitdaColor = palette.GetColor(1);
                    ScottPlot.Color ebitColor = ScottPlot.Color.FromHex("#EAE552");
                    ScottPlot.Color netIncColor = palette.GetColor(2);

                    if (financial.IsEstimate)
                    {
                        revColor = revColor.WithAlpha(40);
                        ebitdaColor = ebitdaColor.WithAlpha(40);
                        ebitColor = ebitColor.WithAlpha(40);
                        netIncColor = netIncColor.WithAlpha(40);
                    }

                    // Track where this year's block begins
                    double yearStartX = currentX;

                    // 1. Revenue Bar
                    double revenue = financial.Revenue / Math.Pow(10, 6);
                    bars.Add(new Bar { Position = currentX, Value = revenue, FillColor = revColor, Size = 0.8 });
                    currentX += 1 + barSpacing;

                    if (financial.Earnings != null)
                    {
                        // 2. EBITDA Bar
                        ebitda = financial.Earnings.EbitdaValue / Math.Pow(10, 6);
                        bars.Add(new Bar { Position = currentX, Value = ebitda, FillColor = ebitdaColor, Size = 0.8 });
                        currentX += 1 + barSpacing;

                        // 3. EBIT Bar
                        ebit = financial.Earnings.EbitValue / Math.Pow(10, 6);
                        bars.Add(new Bar { Position = currentX, Value = ebit, FillColor = ebitColor, Size = 0.8 });
                        currentX += 1 + barSpacing;

                        // 4. Net Income Bar
                        netIncome = financial.Earnings.NetIncomeValue / Math.Pow(10, 6);
                        bars.Add(new Bar { Position = currentX, Value = netIncome, FillColor = netIncColor, Size = 0.8 });
                        currentX += 1 + barSpacing;
                    }

                    // Track where this year's block ends
                    double yearEndX = currentX - (1 + barSpacing);

                    // Calculate the exact visual center of the group for the label
                    double yearCenter = (yearStartX + yearEndX) / 2.0;
                    xAxesYears.Add(new Tick(yearCenter, financial.Year.ToString()));

                    // Add separation before starting the next year group
                    currentX += yearSpacing;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            barArr = bars.ToArray();
            finPlot.Plot.Add.Bars(barArr);
            finPlot.Plot.Legend.FontName = ScottPlot.Fonts.Serif;
            finPlot.Plot.Legend.FontSize = 16;

            //Configure extras
            finPlot.Plot.Legend.ManualItems.Add(new LegendItem
            {
                LabelText = "Revenue",
                FillColor = palette.GetColor(0)
            });
            finPlot.Plot.Legend.ManualItems.Add(new LegendItem
            {
                LabelText = "EBITDA",
                FillColor = palette.GetColor(1)
            });
            finPlot.Plot.Legend.ManualItems.Add(new LegendItem
            {
                LabelText = "EBIT",
                FillColor = ScottPlot.Color.FromHex("#EAE552")
            });
            finPlot.Plot.Legend.ManualItems.Add(new LegendItem
            {
                LabelText = "Net Income",
                FillColor = palette.GetColor(2)
            });
            finPlot.Plot.Legend.IsVisible = true;
            finPlot.Plot.Legend.Alignment = Alignment.UpperLeft;

            finPlot.Plot.Legend.BackgroundColor = ScottPlot.Color.FromHex("#A5CAAF");
            finPlot.Plot.Legend.OutlineColor = ScottPlot.Color.FromHex("#212529");
            finPlot.Plot.Legend.ShadowColor = ScottPlot.Color.FromHex("#A5CAAF").WithOpacity(0.1);
            finPlot.Plot.Legend.ShadowOffset = new(4, 4);
            finPlot.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#272B2F");
            finPlot.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#0e3d54");
            finPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#212529");

            finPlot.Plot.Axes.Color(ScottPlot.Color.FromHex("#A5CAAF"));
            finPlot.Plot.Axes.Bottom.Label.Text = "Year";
            finPlot.Plot.Axes.Bottom.Label.FontSize = 16;
            finPlot.Plot.Axes.Bottom.Label.FontName = "/StockPresentationLib;component/Fonts/#Rubik";
            finPlot.Plot.Axes.Right.Label.FontSize = 16;
            finPlot.Plot.Axes.Right.Label.Text = "Growth (%)";
            finPlot.Plot.Axes.Right.Label.FontName = "/StockPresentationLib;component/Fonts/#Rubik";
            finPlot.Plot.Axes.Left.Label.FontSize = 16;
            finPlot.Plot.Axes.Left.Label.Text = "USD ($MM)";
            finPlot.Plot.Axes.Left.Label.FontName = "/StockPresentationLib;component/Fonts/#Rubik";
            finPlot.Plot.Axes.Title.Label.Bold = true;
            finPlot.Plot.Axes.Title.Label.FontSize = 24;
            finPlot.Plot.Axes.Title.Label.FontName = "/StockPresentationLib;component/Fonts/#Rubik";
            finPlot.Plot.Axes.Title.Label.Text = stockStr;
            finPlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(xAxesYears.ToArray());
            finPlot.Plot.Axes.Bottom.MajorTickStyle.Length = 0;

            // Force ScottPlot to calculate correct data boundaries across all bars
            finPlot.Plot.Axes.AutoScale();

            // Dynamically pad the left and right margins so the edge bars don't clip into the border
            finPlot.Plot.Axes.Margins(bottom: 0, left: 0.1, right: 0.1);

            // Lock it to these exact bounds so it stretches and occupies the total figure area
            var optimalLimits = finPlot.Plot.Axes.GetLimits();
            finPlot.Plot.Axes.SetLimitsX(optimalLimits.XRange.Min, optimalLimits.XRange.Max);

            finPlot.Refresh();
        }       

        public void PlotEbitdaGrowth(bool showPlot)
        {
            if (showPlot)
            {
                List<double> ebitdaGrowth = new List<double>() { 0.0 };
                List<double> positions = new List<double>();

                if (yearlyFinancials != null)
                {
                    for (int i = 0; i < yearlyFinancials.Count() - 1; i++)
                    {
                        if (yearlyFinancials.ElementAt(i).Earnings != null)
                        {
                            double ebitdaPrev = yearlyFinancials.ElementAt(i).Earnings.EbitdaValue;
                            if (ebitdaPrev != 0)
                            {
                                double ebitdaCurr = yearlyFinancials.ElementAt(i + 1).Earnings.EbitdaValue;
                                ebitdaGrowth.Add(100 * ((double)(Math.Abs(ebitdaCurr) - Math.Abs(ebitdaPrev)) / ebitdaPrev));
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
                    scatterEbitdaGrowth.Color = palette.GetColor(1).WithAlpha(scatterLineOpacity);
                    scatterEbitdaGrowth.Axes.YAxis = finPlot.Plot.Axes.Right;
                    scatterEbitdaGrowth.LineWidth = scatterLineWidth;
                    scatterEbitdaGrowth.MarkerSize = scatterMarkerSize;
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

                if (yearlyFinancials != null)
                {
                    for (int i = 0; i < yearlyFinancials.Count() - 1; i++)
                    {
                        double revPrev = yearlyFinancials.ElementAt(i).Revenue;
                        if (revPrev != 0)
                        {
                            double revCurr = yearlyFinancials.ElementAt(i + 1).Revenue;
                            revenueGrowth.Add(100 * ((double)(Math.Abs(revCurr) - Math.Abs(revPrev)) / revPrev));
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
                scatterRevGrowth.Color = palette.GetColor(0).WithAlpha(scatterLineOpacity);
                scatterRevGrowth.Axes.YAxis = finPlot.Plot.Axes.Right;
                scatterRevGrowth.LineWidth = scatterLineWidth;
                scatterRevGrowth.MarkerSize = scatterMarkerSize;
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

                if (yearlyFinancials != null)
                {
                    for (int i = 0; i < yearlyFinancials.Count() - 1; i++)
                    {
                        double ebitPrev = yearlyFinancials.ElementAt(i).Earnings.EbitValue;
                        if (ebitPrev != 0)
                        {
                            double ebitCurr = yearlyFinancials.ElementAt(i + 1).Earnings.EbitValue;
                            ebitGrowth.Add(100 * ((double)(Math.Abs(ebitCurr) - Math.Abs(ebitPrev)) / ebitPrev));
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
                scatterEbitGrowth.Color = ScottPlot.Color.FromHex("#EAE552").WithAlpha(scatterLineOpacity);
                scatterEbitGrowth.Axes.YAxis = finPlot.Plot.Axes.Right;
                scatterEbitGrowth.LineWidth = scatterLineWidth;
                scatterEbitGrowth.MarkerSize = scatterMarkerSize;
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

                if (yearlyFinancials != null)
                {
                    for (int i = 0; i < yearlyFinancials.Count() - 1; i++)
                    {
                        double nIncPrev = yearlyFinancials.ElementAt(i).Earnings.NetIncomeValue;
                        if (nIncPrev != 0)
                        {
                            double incCurr = yearlyFinancials.ElementAt(i + 1).Earnings.NetIncomeValue;
                            nIncomeGrowth.Add(100 * ((double)(Math.Abs(incCurr) - Math.Abs(nIncPrev)) / nIncPrev));
                        }
                        else
                        {
                            nIncomeGrowth.Add(0.0);
                        }

                        positions.Add(xAxesYears[i].Position);
                    }
                    positions.Add(xAxesYears.Last().Position);

                    scatterNetIncGrowth = finPlot.Plot.Add.Scatter(positions, nIncomeGrowth);
                    scatterNetIncGrowth.Color = palette.GetColor(2).WithAlpha(scatterLineOpacity);
                    scatterNetIncGrowth.Axes.YAxis = finPlot.Plot.Axes.Right;
                    scatterNetIncGrowth.LineWidth = scatterLineWidth;
                    scatterNetIncGrowth.MarkerSize = scatterMarkerSize;
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
