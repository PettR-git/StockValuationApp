using ScottPlot.Plottables;
using ScottPlot.WPF;
using ScottPlot;
using StockValuationApp.Entities.Stocks.Metrics;
using StockValuationApp.Entities.Stocks;
using StockValuationApp.Main.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockValuationApp.Entities.Enums;
using System.Drawing;
using ScottPlot.Statistics;
using System.Windows.Media;

namespace StockPresentationLib.Plot
{
    public class PlotReturns
    {
        private List<Tick> xAxesYears;
        private List<YearlyFinancials> yearlyFinancials;
        private Stock stock;
        private WpfPlot finPlot;
        private List<double> roeVals;
        private List<double> roicVals;
        private List<double> fcfVals;
        private Scatter scatterRoeGrowth;
        private Scatter scatterRoicGrowth;
        private Scatter scatterFcfGrowth;
        private ScottPlot.Palettes.Category10 palette;
        public PlotReturns(WpfPlot finPlot, Stock stock)
        {
            this.finPlot = finPlot;
            this.stock = stock;
            yearlyFinancials = stock.Financials;
            palette = new ScottPlot.Palettes.Category10();
            roeVals = new List<double>();
            roicVals = new List<double>();  
            fcfVals = new List<double>();
        }

        public void PlotRoeRoicEvFcf()
        {
            //Bar graph and Scatter line growth

            List<Bar> bars = new List<Bar>();
            xAxesYears = new List<Tick>();

            for (int i = 0, indexPos = 0, indexBar = 0, posTick = 0; i < yearlyFinancials.Count(); i++, indexPos ++, indexBar++, posTick += 4)
            {
                try
                {
                    if (i < yearlyFinancials.Count())
                    {
                        double roe = 0, roic = 0, fcf = 0;

                        if (yearlyFinancials[i].KeyFiguresDict != null)
                        {
                            var keyFigureDict = yearlyFinancials[i].KeyFiguresDict;

                            if(keyFigureDict.TryGetValue(KeyFigureTypes.ReturnOnEquity, out decimal returnOnEquity))
                            {
                                roe = (double)returnOnEquity;
                                roeVals.Add(roe);
                            }
                            bars.Add(new Bar { Position = indexPos++, Value = roe, FillColor = palette.GetColor(0) });

                            if (keyFigureDict.TryGetValue(KeyFigureTypes.ReturnOnInvCap, out decimal returnOnInvCap))
                            {
                                roic = (double)returnOnInvCap;
                                roicVals.Add(roic);
                            }
                            bars.Add(new Bar { Position = indexPos++, Value = roic, FillColor = palette.GetColor(1) });

                             if(keyFigureDict.TryGetValue(KeyFigureTypes.EvFreecashflow, out decimal returnEvFreecashflow))
                             {
                                 fcf = (double)returnEvFreecashflow;
                                 fcfVals.Add(fcf);
                             }
                            bars.Add(new Bar { Position = indexPos++, Value = fcf, FillColor = palette.GetColor(2) });
                        }
                        xAxesYears.Add(new Tick(posTick, yearlyFinancials[i].Year.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            finPlot.Plot.Add.Bars(bars);
            finPlot.Plot.Legend.FontName = ScottPlot.Fonts.Serif;
            finPlot.Plot.Legend.FontSize = 16;
            //Configure extras
            finPlot.Plot.Legend.ManualItems.Add(new LegendItem
            {
                LabelText = "ROE",
                FillColor = palette.GetColor(0)
            });
            finPlot.Plot.Legend.ManualItems.Add(new LegendItem
            {
                LabelText = "ROIC",
                FillColor = palette.GetColor(1),
            });
            finPlot.Plot.Legend.ManualItems.Add(new LegendItem
            {
                LabelText = "EV/FCF",
                FillColor = palette.GetColor(2),
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
            finPlot.Plot.Axes.Right.Label.Text = "Growth (%)";
            finPlot.Plot.Axes.Left.Label.Text = "ROE/ROIC (%)";
            finPlot.Plot.Axes.Title.Label.FontSize = 24;
            finPlot.Plot.Axes.Bottom.Label.FontName = "/StockPresentationLib;component/Fonts/#Rubik";
            finPlot.Plot.Axes.Right.Label.FontName = "/StockPresentationLib;component/Fonts/#Rubik";
            finPlot.Plot.Axes.Left.Label.FontName = "/StockPresentationLib;component/Fonts/#Rubik";
            finPlot.Plot.Axes.Title.Label.FontName = "/StockPresentationLib;component/Fonts/#Rubik";
            finPlot.Plot.Axes.Title.Label.Bold = true;
            finPlot.Plot.Axes.Bottom.Label.FontSize = 16;
            finPlot.Plot.Axes.Left.Label.FontSize = 16;
            finPlot.Plot.Axes.Right.Label.FontSize = 16;
            finPlot.Plot.Axes.Title.Label.Text = stock.ToString();
            finPlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(xAxesYears.ToArray());
            finPlot.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            finPlot.Plot.Axes.Margins(bottom: 0, left: 0.7, right: 0.7);

            finPlot.Refresh();
        }

        public void PlotRoeGrowth(bool showPlot)
        {
            if (showPlot)
            {
                List<double> roeGrowth = new List<double>() { 0.0 };
                List<double> positions = new List<double>();

                if (roeVals.Count > 0)
                {
                    double currRoe = 0, prevRoe = 0;

                    for (int i = 0; i < roeVals.Count - 1; i++)
                    {
                        prevRoe = roeVals[i];

                        if (prevRoe != 0)
                        {
                            currRoe = roeVals[i + 1];
                            roeGrowth.Add(100 * (Math.Abs(currRoe) - Math.Abs(prevRoe)) / prevRoe);
                        }
                        else
                        {
                            roeGrowth.Add(0.0);
                        }

                        positions.Add(xAxesYears[i].Position);
                    }

                    positions.Add(xAxesYears.Last().Position);

                    scatterRoeGrowth = finPlot.Plot.Add.Scatter(positions, roeGrowth);
                    scatterRoeGrowth.Color = palette.GetColor(0);
                    scatterRoeGrowth.Axes.YAxis = finPlot.Plot.Axes.Right;
                }
            }
            else
            {
                finPlot.Plot.Remove(scatterRoeGrowth);
            }

            finPlot.Refresh();
        }

        public void PlotRoicGrowth(bool showPlot)
        {
            if (showPlot)
            {
                List<double> roicGrowth = new List<double>() { 0.0 };
                List<double> positions = new List<double>();

                if (roicVals.Count > 0)
                {
                    double currRoic = 0, prevRoic = 0;

                    for (int i = 0; i < roicVals.Count - 1; i++)
                    {
                        prevRoic = roicVals[i];

                        if (prevRoic != 0)
                        {
                            currRoic = roicVals[i + 1];
                            roicGrowth.Add(100 * (Math.Abs(currRoic) - Math.Abs(prevRoic)) / prevRoic);
                        }
                        else
                        {
                            roicGrowth.Add(0.0);
                        }

                        positions.Add(xAxesYears[i].Position);
                    }

                    positions.Add(xAxesYears.Last().Position);

                    scatterRoicGrowth = finPlot.Plot.Add.Scatter(positions, roicGrowth);
                    scatterRoicGrowth.Color = palette.GetColor(1);
                    scatterRoicGrowth.Axes.YAxis = finPlot.Plot.Axes.Right;
                }
            }
            else
            {
                finPlot.Plot.Remove(scatterRoicGrowth);
            }

            finPlot.Refresh();
        }

        public void PlotFcfGrowth(bool showPlot)
        {
            if (showPlot)
            {
                List<double> fcfGrowth = new List<double>() { 0.0 };
                List<double> positions = new List<double>();

                if (fcfVals.Count > 0)
                {
                    double currFcf = 0, prevFcf = 0;

                    for (int i = 0; i < fcfVals.Count - 1; i++)
                    {
                        prevFcf = fcfVals[i];

                        if (prevFcf != 0)
                        {
                            currFcf = fcfVals[i + 1];
                            fcfGrowth.Add(100 * (Math.Abs(currFcf) - Math.Abs(prevFcf)) / prevFcf);
                        }
                        else
                        {
                            fcfGrowth.Add(0.0);
                        }

                        positions.Add(xAxesYears[i].Position);
                    }

                    positions.Add(xAxesYears.Last().Position);

                    scatterFcfGrowth = finPlot.Plot.Add.Scatter(positions, fcfGrowth);
                    scatterFcfGrowth.Color = palette.GetColor(2);
                    scatterFcfGrowth.Axes.YAxis = finPlot.Plot.Axes.Right;
                }
            }
            else
            {
                finPlot.Plot.Remove(scatterFcfGrowth);
            }

            finPlot.Refresh();
        }
    }
}
