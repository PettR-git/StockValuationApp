using ScottPlot.Plottables;
using ScottPlot;
using ScottPlot.WPF;
using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPresentationLib.Plot
{
    public class PlotCriterias
    {
        private Stock stock;
        private List<ScottPlot.RadarSeries>? valuationSeries;

        public PlotCriterias(Stock stock)
        {
            this.stock = stock;
        }

        public void PlotValuation(WpfPlot valPlot)
        {
            if (stock.StockScore == null)
            {
                return;
            }

            //Revgrowth Score
            valuationSeries = new List<ScottPlot.RadarSeries>()
            {
                new ScottPlot.RadarSeries() {Values = [
                    stock.StockScore.RevGrowthScore,
                    stock.StockScore.EpsGrowthScore,
                    stock.StockScore.EvEbitScore,
                    stock.StockScore.EvFcfScore,
                    stock.StockScore.RoeRoicScore,
                ],
                    FillColor = ScottPlot.Color.FromHex("#A5CAAF").WithAlpha(.5)
                }
            };

            var valHexagon = valPlot.Plot.Add.Radar(valuationSeries);

            valHexagon.LineColor = ScottPlot.Color.FromHex("#A5CAAF");
            valHexagon.Labels = new string[] { "Revenue CAGR (5yrs)", "Eps CAGR (5yrs)", "EV/EBIT", "EV/FCF", "ROE & ROIC" }
                .Select(s => new Label()
                {
                    Text = s,
                    Alignment = Alignment.MiddleCenter,
                    ForeColor = ScottPlot.Color.FromHex("#A5CAAF"),
                    FontSize = 16,
                    FontName = Fonts.Serif
                })
                .ToArray();

            valPlot.Plot.Title("Valuation Strength Mapping", 16);
            valPlot.Plot.Axes.Title.Label.FontName = "/StockPresentationLib;component/Fonts/#Rubik";
            valPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#212529");
            valPlot.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#212529");
            valPlot.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#0e3d54");
            valPlot.Plot.Axes.Frameless();
            valPlot.Plot.Axes.Margins(0.5, 0.5);
            valPlot.Plot.ShowLegend();
            valPlot.Plot.HideGrid();
        }

        public void PlotMoat(WpfPlot moatPlot)
        {
            if (stock.StockScore == null)
            {
                return;
            }

            valuationSeries = new List<ScottPlot.RadarSeries>()
            {
                new ScottPlot.RadarSeries() {Values = [
                    stock.StockScore.NetworkEffectScore,
                    stock.StockScore.CostAdvScore,
                    stock.StockScore.SwitchCostScore,
                    stock.StockScore.ScalabilityScore,
                    stock.StockScore.IntangAssetScore
                ],
                    FillColor = ScottPlot.Color.FromHex("#A5CAAF").WithAlpha(.5)
                }
            };

            var valHexagon = moatPlot.Plot.Add.Radar(valuationSeries);

            valHexagon.LineColor = ScottPlot.Color.FromHex("#A5CAAF");
            valHexagon.Labels = new string[] { "Network Effects", "Cost Advantages", "Switching Costs", "Scalability", "Intangible Assets" }
                .Select(s => new Label()
                {
                    Text = s,
                    Alignment = Alignment.MiddleCenter,
                    ForeColor = ScottPlot.Color.FromHex("#A5CAAF"),
                    FontSize = 16,
                    FontName = Fonts.Serif
                })
                .ToArray();

            moatPlot.Plot.Title("Moat Strength Mapping", 16);
            moatPlot.Plot.Axes.Title.Label.FontName = "/StockPresentationLib;component/Fonts/#Rubik";
            moatPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#212529");
            moatPlot.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#212529");
            moatPlot.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#0e3d54");
            moatPlot.Plot.Axes.Frameless();
            moatPlot.Plot.Axes.Margins(0.5, 0.5);
            moatPlot.Plot.ShowLegend();
            moatPlot.Plot.HideGrid();
        }

        public void PlotUnderParam(WpfPlot marketPlot)
        {
            if (stock.StockScore == null)
            {
                return;
            }

            valuationSeries = new List<ScottPlot.RadarSeries>()
            {
                new ScottPlot.RadarSeries() {Values = [
                    stock.StockScore.SectorGrowthScore,
                    stock.StockScore.ConsensusScore,
                    stock.StockScore.NonDisruptiveScore,
                    stock.StockScore.MarginExpScore,
                    stock.StockScore.MarketVolatilityScore
                ],
                    FillColor = ScottPlot.Color.FromHex("#A5CAAF").WithAlpha(.5)
                }
            };

            var valHexagon = marketPlot.Plot.Add.Radar(valuationSeries);

            valHexagon.LineColor = ScottPlot.Color.FromHex("#A5CAAF");
            valHexagon.Labels = new string[] { "Sector Growth", "Consensus", "Non-Disruptive Sector", "Margin Expansion", "Sector Volatility" }
                .Select(s => new Label()
                {
                    Text = s,
                    Alignment = Alignment.MiddleCenter,
                    ForeColor = ScottPlot.Color.FromHex("#A5CAAF"),
                    FontSize = 16,
                    FontName = Fonts.Serif
                })
                .ToArray();

            marketPlot.Plot.Title("Market Strength Mapping", 16);
            marketPlot.Plot.Axes.Title.Label.FontName = "/StockPresentationLib;component/Fonts/#Rubik";
            marketPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#212529");
            marketPlot.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#212529");
            marketPlot.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#0e3d54");
            marketPlot.Plot.Axes.Frameless();
            marketPlot.Plot.Axes.Margins(0.5, 0.5);
            marketPlot.Plot.ShowLegend();
            marketPlot.Plot.HideGrid();
        }
    }
}
