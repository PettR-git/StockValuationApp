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

            this.DataContextChanged += Returns_Changed;
        }

        private void Returns_Changed(object sender, DependencyPropertyChangedEventArgs e)
        {
            Stock stock = null;

            if (this.DataContext is ReturnsVM returnsVM)
            {
                if (returnsVM.Stock != null)
                {              
                    stock = returnsVM.Stock;
                    plotReturns = new PlotReturns(WpfPlot2, stock);
                    PlotAllReturns();

                    if (returnsVM.FirstPlot == true)
                    {
                        returnsVM.FirstPlot = false;
                        SetInitialCbxValues();
                    }
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
            plotReturns.PlotRoeRoicEvFcf();
        }

        private void PlotRoic()
        {
        }

        private void cbxRoeGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ReturnsVM returnsVM)
            {
                if (cbxRoeGrowth.IsChecked == true)
                {
                    returnsVM.CbxRoeGrwthChecked = true;
                    plotReturns.PlotRoeGrowth(true);
                }
                else
                {
                    returnsVM.CbxRoeGrwthChecked = false;
                    plotReturns.PlotRoeGrowth(false);
                }
            }
        }

        private void cbxRoicGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ReturnsVM returnsVM)
            {
                if (cbxRoicGrowth.IsChecked == true)
                {
                    returnsVM.CbxRoicGrwthChecked = true;
                    plotReturns.PlotRoicGrowth(true);
                }
                else
                {
                    returnsVM.CbxRoicGrwthChecked = false;
                    plotReturns.PlotRoicGrowth(false);
                }
            }
        }

        private void cbxFcfGrowth_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ReturnsVM returnsVM)
            {
                if (cbxFcfGrowth.IsChecked == true)
                {
                    returnsVM.CbxFcfGrwthChecked = true;
                    plotReturns.PlotFcfGrowth(true);
                }
                else
                {
                    returnsVM.CbxFcfGrwthChecked = false;
                    plotReturns.PlotFcfGrowth(false);
                }
            }
        }
    }
}
