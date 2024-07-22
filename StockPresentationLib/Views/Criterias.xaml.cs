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
    /// Interaction logic for Criterias.xaml
    /// </summary>
    public partial class Criterias : UserControl
    {
        private PlotCriterias plotCriterias;
        public Criterias()
        {
            InitializeComponent();

            this.DataContextChanged += Returns_Changed;
        }

        private void Returns_Changed(object sender, DependencyPropertyChangedEventArgs e)
        {
            Stock stock = null;

            if (this.DataContext is CriteriasVM criteriasVM)
            {
                if (criteriasVM.Stock != null)
                {
                    stock = criteriasVM.Stock;
                    plotCriterias = new PlotCriterias(stock);

                    plotCriterias.PlotValuation(WpfPlotValuation);
                }
            }
        }
    }
}
