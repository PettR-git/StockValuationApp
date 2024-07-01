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
    /// Interaction logic for Returns.xaml
    /// </summary>
    public partial class Returns : UserControl
    {
        private PlotReturns plotReturns;
        public Returns()
        {
            InitializeComponent();

            this.Loaded += Returns_Loaded;
        }

        private void Returns_Loaded(object sender, RoutedEventArgs e)
        {
            Stock stock = null;

            if (this.DataContext is ReturnsVM returnsVM)
            {
                stock = returnsVM.Stock;

                if (stock != null)
                {
                    plotReturns = new PlotReturns(WpfPlot2, stock);
                    PlotAllReturns();
                    SetInitialCbxValues();
                }
            }
        }

        private void SetInitialCbxValues()
        {
            cbxFcfGrowth.IsChecked = true;
            cbxRoicGrowth.IsChecked = true;
            cbxRoeGrowth.IsChecked = true;    
        }

        public void PlotAllReturns()
        {
            plotReturns.PlotAllReturns();
        }

        private void PlotRoic()
        {
        }

        private void cbxRoeGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (cbxRoeGrowth.IsChecked == true)
                plotReturns.PlotRoeGrowth(true);
            else
                plotReturns.PlotRoeGrowth(false);
        }

        private void cbxRoicGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (cbxRoicGrowth.IsChecked == true)
                plotReturns.PlotRoicGrowth(true);
            else
                plotReturns.PlotRoicGrowth(false);
        }

        private void cbxFcfGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (cbxFcfGrowth.IsChecked == true)
                plotReturns.PlotFcfGrowth(true);
            else
                plotReturns.PlotFcfGrowth(false);
        }
    }
}
