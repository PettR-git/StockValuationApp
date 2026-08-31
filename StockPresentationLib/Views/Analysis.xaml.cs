using System.Windows;
using System.Windows.Controls;
using StockPresentationLib.ViewModel;

namespace StockPresentationLib.Views
{
    public partial class Analysis : UserControl
    {
        public Analysis()
        {
            InitializeComponent();

            this.DataContextChanged += Analysis_DataContextChanged;
        }

        private void Analysis_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is AnalysisVM analysisVM)
            {
                // Candlestick chart will be bound via XAML bindings
            }
        }
    }
}