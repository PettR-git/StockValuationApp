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

namespace StockPresentationLib.Plot
{
    public class PlotReturns
    {
        private List<Tick> xAxesYears;
        private List<YearlyFinancials> yearlyFinancials;
        private Stock stock;
        private WpfPlot finPlot;
        private Scatter scatterRoeGrowth;
        private Scatter scatterRoicGrowth;
        private Scatter scatterFfcGrowth;
        private ScottPlot.Palettes.Category10 palette;
        public PlotReturns(WpfPlot finPlot, Stock stock)
        {
            this.finPlot = finPlot;
            this.stock = stock;
            yearlyFinancials = stock.Financials;
            palette = new ScottPlot.Palettes.Category10();
        }

        public void PlotAllReturns()
        {
            //Bar graph and Scatter line growth

            List<Bar> bars = new List<Bar>();
            xAxesYears = new List<Tick>();

            for (int i = 0, indexPos = 0, indexBar = 0, posTick = 0; i < yearlyFinancials.Count(); i++, indexPos += 2, indexBar++, posTick += 4)
            {
                try
                {
                    if (i < yearlyFinancials.Count())
                    {
                        double roe = 0, roic = 0, fcf = 0;
                        double revenue = yearlyFinancials.ElementAt(i).Revenue;

                        bars.Add(new Bar { Position = indexPos, Value = revenue, FillColor = palette.GetColor(0) });

                        if (yearlyFinancials[i].KeyFiguresDict != null)
                        {
                            var keyFigureDict = yearlyFinancials[i].KeyFiguresDict;

                            if(keyFigureDict.TryGetValue(KeyFigureTypes.ReturnOnEquity, out double returnOnEquity))
                            {
                                roe = returnOnEquity;
                                bars.Add(new Bar { Position = ++indexPos, Value = roe, FillColor = palette.GetColor(0) });
                            }

                            if (keyFigureDict.TryGetValue(KeyFigureTypes.ReturnOnInvCap, out double returnOnInvCap))
                            {
                                roic = returnOnInvCap;
                                bars.Add(new Bar { Position = ++indexPos, Value = roic, FillColor = palette.GetColor(1) });
                            }

                            if(keyFigureDict.TryGetValue(KeyFigureTypes.EvFreecashflow, out double returnEvFreecashflow))
                            {
                                fcf = returnEvFreecashflow;
                                bars.Add(new Bar { Position = ++indexPos, Value = roic, FillColor = palette.GetColor(2) });
                            }
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

            //Configure extras
            finPlot.Plot.Legend.ManualItems.Add(new LegendItem
            {
                LabelText = "ROE",
                FillColor = palette.GetColor(0)
            });
            finPlot.Plot.Legend.ManualItems.Add(new LegendItem
            {
                LabelText = "ROIC",
                FillColor = palette.GetColor(1)
            });
            finPlot.Plot.Legend.ManualItems.Add(new LegendItem
            {
                LabelText = "EV/FCF",
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
            finPlot.Plot.Axes.Right.Label.Text = "Growth (%)";
            finPlot.Plot.Axes.Left.Label.Text = "Metric Value";
            finPlot.Plot.Axes.Title.Label.Text = stock.ToString();
            finPlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(xAxesYears.ToArray());
            finPlot.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            finPlot.Plot.Axes.Margins(bottom: 0, left: 0.7, right: 0.7);

            finPlot.Refresh();
        }

        public void PlotRoeGrowth(bool showPlot)
        {

        }

        public void PlotRoicGrowth(bool showPlot)
        {

        }

        public void PlotFcfGrowth(bool showPlot)
        {

        }


        /*  public void PlotEbitdaGrowth(bool showPlot)
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
                      scatterEbitdaGrowth.Color = palette.GetColor(1);
                      scatterEbitdaGrowth.Axes.YAxis = finPlot.Plot.Axes.Right;
                  }
              }
              else
              {
                  finPlot.Plot.Remove(scatterEbitdaGrowth);
              }

              finPlot.Refresh();
          }*/

    }
}
