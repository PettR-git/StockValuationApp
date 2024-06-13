using ScottPlot;
using ScottPlot.Plottables;
using StockValuationApp.Entities.Stocks.Metrics;
using StockValuationApp.Entities.Stocks.Metrics.Earnings;
using StockValuationApp.Main.Enums;
using StockValuationApp.Main.Plot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = ScottPlot.Color;

namespace StockValuationApp.Windows
{
    /// <summary>
    /// Interaction logic for StockPlotWindow.xaml
    /// </summary>
    public partial class StockPlotWindow : Window
    {
        private PlotStockMetrics plotStockMetrics;

        public StockPlotWindow(List<YearlyFinancials> yf)
        {
            InitializeComponent();

            plotStockMetrics = new PlotStockMetrics(WpfPlot1, yf);
            PlotRevenueAndEarnings();
            SetInitialCbxValues();
        } 

        private void SetInitialCbxValues()
        {
            cbxEbitGrowth.IsChecked = true;   
            cbxNetIncGrowth.IsChecked = true;
        }

        private void PlotRevenueAndEarnings()
        {
            plotStockMetrics.PlotRevenueAndEarnings();
        }

        public void RenderPlot()
        {
            plotStockMetrics.RenderPlot();
        }

        public void PlotRoe()
        {

        }

        private void PlotRoic()
        {

        }

        private void cbxEbitdaGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if(cbxEbitdaGrowth.IsChecked == true)
                plotStockMetrics.PlotEbitdaGrowth(true);
            else
                plotStockMetrics.PlotEbitdaGrowth(false);
        }

        private void cbxEbitGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (cbxEbitGrowth.IsChecked == true)
                plotStockMetrics.PlotEbitGrowth(true);
            else
                plotStockMetrics.PlotEbitGrowth(false);
        }

        private void cbxRevGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (cbxRevGrowth.IsChecked == true)
                plotStockMetrics.PlotRevenueGrowth(true);
            else
                plotStockMetrics.PlotRevenueGrowth(false);
        }

        private void cbxNetIncGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (cbxNetIncGrowth.IsChecked == true)
                plotStockMetrics.PlotNetIncomeGrowth(true);
            else
                plotStockMetrics.PlotNetIncomeGrowth(false);
        }
    }
}
