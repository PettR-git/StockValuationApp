using StockPresentationLib.Plot;
using StockPresentationLib.ViewModel;
using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace StockPresentationLib.Views
{
    /// <summary>
    /// Interaction logic for Fundamentals.xaml
    /// </summary>
    public partial class Fundamentals : UserControl
    {
        private PlotCriterias plotCriterias;
        public Fundamentals()
        {
            InitializeComponent();

            this.DataContextChanged += Returns_Changed;
        }

        private void Returns_Changed(object sender, DependencyPropertyChangedEventArgs e)
        {
            Stock stock = null;

            if (this.DataContext is FundamentalsVM fundamentalsVM)
            {
                if (fundamentalsVM.Stock != null)
                {
                    stock = fundamentalsVM.Stock;
                    plotCriterias = new PlotCriterias(fundamentalsVM.Stock);

                    plotCriterias.PlotValuation(WpfPlotValuation);
                    plotCriterias.PlotMoat(WpfPlotMoat);
                    plotCriterias.PlotUnderParam(WpfPlotUnderParam);

                    tbValScore.Text = "Score: " + stock.StockScore?.GetValuationScore.ToString() + "/10";
                    tbMoatScore.Text = "Score: " + stock.StockScore?.GetMoatScore.ToString() + "/10";
                    tbMarketScore.Text = "Score: " + stock.StockScore?.GetMarketScore.ToString() + "/10";
                }
            }
        }

        private void btnAddMoats_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddMarketData_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
