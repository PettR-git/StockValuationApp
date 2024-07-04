using StockPresentationLib.Plot;
using StockPresentationLib.ViewModel;
using StockValuationApp.Entities.Stocks;
using StockValuationApp.Entities.Stocks.Metrics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockPresentationLib.Views
{
    /// <summary>
    /// Interaction logic for Earnings.xaml
    /// </summary>
    public partial class Earnings : UserControl
    {
        private PlotEarnings plotEarnings;
        public Earnings()
        {
            InitializeComponent();

            this.DataContextChanged += Earnings_DataContextChanged;
        }

        private void Earnings_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is EarningsVM earningsVM)
            {
                if (earningsVM.Stock != null)
                {
                    if(earningsVM.PlotEarnings == null || earningsVM.Stock != earningsVM.PlotEarnings.PlotStock)
                    {
                        bool newPlot = true;

                        if(earningsVM.PlotEarnings?.PlotStock != null)
                            newPlot = false;

                        plotEarnings = new PlotEarnings(WpfPlot1, earningsVM.Stock);
                        earningsVM.PlotEarnings = plotEarnings;
                        PlotRevenueAndEarnings();

                        if(newPlot)
                            SetInitialCbxValues();
                    }
                    else
                    {
                        plotEarnings = earningsVM.PlotEarnings;
                        plotEarnings.UpdatePlotAndStock(WpfPlot1, earningsVM.Stock);
                        PlotRevenueAndEarnings();
                    }
                }
            }
        }

        private void SetInitialCbxValues()
        {
            cbxEbitGrowth.IsChecked = true;
            cbxNetIncGrowth.IsChecked = true;
        }

        private void PlotRevenueAndEarnings()
        {
            plotEarnings.PlotRevenueAndEarnings();
        }

        public void RenderPlot()
        {
            plotEarnings.RenderPlot();
        }

        private void cbxEbitdaGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is EarningsVM earningsVM)
            {
                if (cbxEbitdaGrowth.IsChecked == true)
                {
                    plotEarnings.PlotEbitdaGrowth(true);
                    earningsVM.CbxEbitdaGrwthChecked = true;
                }
                else
                {
                    plotEarnings.PlotEbitdaGrowth(false);
                    earningsVM.CbxEbitdaGrwthChecked = false;
                }
            }
        }

        private void cbxEbitGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is EarningsVM earningsVM)
            {
                if (cbxEbitGrowth.IsChecked == true)
                {
                    plotEarnings.PlotEbitGrowth(true);           
                    earningsVM.CbxEbitGrwthChecked= true;
                }
                else
                {
                    plotEarnings.PlotEbitGrowth(false);
                    earningsVM.CbxEbitGrwthChecked = false;
                }
            }
        }

        private void cbxRevGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is EarningsVM earningsVM)
            {
                if (cbxRevGrowth.IsChecked == true)
                {
                    plotEarnings.PlotRevenueGrowth(true);
                    earningsVM.CbxRevGrwthChecked = true;
                }                   
                else
                {
                    plotEarnings.PlotRevenueGrowth(false);
                    earningsVM.CbxRevGrwthChecked=false;
                }                
            }
        }

        private void cbxNetIncGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is EarningsVM earningsVM)
            {
                if (cbxNetIncGrowth.IsChecked == true)
                {
                    plotEarnings.PlotNetIncomeGrowth(true);
                    earningsVM.CbxNetIncGrwthChecked = true;
                }
                else
                {
                    plotEarnings.PlotNetIncomeGrowth(false);
                    earningsVM.CbxNetIncGrwthChecked= false;
                }
            }
        }
    }
}
