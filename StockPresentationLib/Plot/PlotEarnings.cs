﻿using ScottPlot;
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
            //Bar graph and Scatter line growth            
            List<Bar> bars = new List<Bar>();
            xAxesYears = new List<Tick>();

            for (int i = 0, indexPos = 0, indexBar = 0, posTick = 0; i < yearlyFinancials.Count(); i++, indexPos += 2, indexBar++, posTick += 5)
            {
                try
                {
                    if (i < yearlyFinancials.Count())
                    {
                        double ebit = 0, ebitda = 0, netIncome = 0;
                        double revenue = yearlyFinancials.ElementAt(i).Revenue;

                        ScottPlot.Color revColor = palette.GetColor(0);
                        ScottPlot.Color ebitdaColor = palette.GetColor(1);
                        ScottPlot.Color ebitColor = ScottPlot.Color.FromHex("#EAE552");
                        ScottPlot.Color netIncColor = palette.GetColor(2);

                        if (yearlyFinancials[i].IsEstimate == true)
                        {
                            revColor = revColor.WithAlpha(40);
                            ebitdaColor = ebitdaColor.WithAlpha(40);
                            ebitColor = ebitColor.WithAlpha(40);
                            netIncColor = netIncColor.WithAlpha(40);
                        }

                        bars.Add(new Bar { Position = indexPos, Value = revenue, FillColor = revColor });

                        if (yearlyFinancials.ElementAt(i).Earnings != null)
                        {
                            ebitda = yearlyFinancials.ElementAt(i).Earnings.EbitdaValue;
                            bars.Add(new Bar { Position = ++indexPos, Value = ebitda, FillColor = ebitdaColor });

                            ebit = yearlyFinancials.ElementAt(i).Earnings.EbitValue;
                            bars.Add(new Bar { Position = ++indexPos, Value = ebit, FillColor = ebitColor });

                            netIncome = yearlyFinancials.ElementAt(i).Earnings.NetIncomeValue;
                            bars.Add(new Bar { Position = ++indexPos, Value = netIncome, FillColor = netIncColor });
                        }
                        xAxesYears.Add(new Tick(posTick, yearlyFinancials.ElementAt(i).Year.ToString()));
                    }
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
            finPlot.Plot.Axes.Margins(bottom: 0, left: 0.7, right: 0.7);

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
                    scatterEbitdaGrowth.Color = palette.GetColor(1);
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
                scatterRevGrowth.Color = palette.GetColor(0);
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
                scatterEbitGrowth.Color = ScottPlot.Color.FromHex("#EAE552");
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
                    scatterNetIncGrowth.Color = palette.GetColor(2);
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
