using StockValuationApp.Entities.Enums;
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

namespace StockValuationApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StockManager stockManager;
        private StockInfoWindow stockInfoWindow;
        public MainWindow()
        {
            InitializeComponent();
            InitializeGUI();
            stockManager = new StockManager();
            stockManager.AddStockTestValues();
            UpdateStockUI();
        }

        //Initialize UI
        private void InitializeGUI()
        {
            MetricType[] metricTypes = (MetricType[])Enum.GetValues(typeof(MetricType));
            string metricStr = string.Empty;

            foreach(MetricType metricType in metricTypes)
            {
                switch (metricType)
                {
                    case MetricType.EvEbitda:
                        metricStr = "EV/EBITDA";
                        break;
                    case MetricType.EvEbit:
                        metricStr = "EV/EBIT";
                        break;
                    case MetricType.PriceToEarnings:
                        metricStr = "P/E";
                        break;
                    case MetricType.NetDebtToEbitda:
                        metricStr = "Net Debt/EBITDA";
                        break;
                    default:
                        Console.WriteLine("Incorrect metrictype");
                        break;
                }
                cmbMetrics.Items.Add(metricStr);
            }         
        }

        /// <summary>
        /// Retrieve index from selected item in listview, get corresponding stock object
        /// And visualize all metrics data for the object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalculateValuation_Click(object sender, RoutedEventArgs e)
        {
            int index = lvwAllStocks.SelectedIndex;
            Stock stock = stockManager.getListItemAt(index);

            if (index == -1 || stock == null || cmbMetrics.SelectedItem == null)
            {
               MessageBox.Show("Choose a stock from stock list to calculate", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string metricStr = (string)cmbMetrics.SelectedItem;
            MetricType metricType = MetricType.EvEbitda;

            switch (metricStr)
            {
                case "EV/EBITDA":
                    metricType = MetricType.EvEbitda;
                    break;
                case "EV/EBIT":
                    metricType = MetricType.EvEbit;
                    break;
                case "P/E":
                    metricType = MetricType.PriceToEarnings;
                    break;
                case "Net Debt/EBITDA":
                    metricType = MetricType.NetDebtToEbitda;
                    break;
                default:
                    Console.WriteLine("Metrictype not added to switch");
                    break;
            }
 
            stockInfoWindow = new StockInfoWindow(metricType);
            stockInfoWindow.MetricsGiven += OnGetMetricsData;
            stockInfoWindow.ShowDialog();

            UpdateFinancialUI(stock);
        }

        /// <summary>
        /// Method as a subscriber to metric event
        /// To add metricsdata from event args
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">holds metric data for stock</param>
        private void OnGetMetricsData(object sender, MetricEventArgs e)
        {
            int index = lvwAllStocks.SelectedIndex;
            e.Stock = stockManager.getListItemAt(index);

            if (!stockManager.AddMetricDataFrStock(e))
                MessageBox.Show("Error in adding metrics data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Update the stocks listview
        /// </summary>
        private void UpdateStockUI()
        {
            Stock stock = null;
            lvwAllStocks.Items.Clear();

            for(int i = 0; i<stockManager.Count(); i++)
            {
                stock = stockManager.getListItemAt(i);
                lvwAllStocks.Items.Add(stock.ToString());
            }
        }

        /// <summary>
        /// Update metricsListview for a stocks metric data
        /// </summary>
        /// <param name="stock"></param>
        private void UpdateFinancialUI(Stock stock)
        {
            lvwStockInfo.Items.Clear();
            lvwStockInfo.Items.Add(stock.ToString());

            foreach (var fin in stock.Financials)
            {
                if (fin != null)
                    lvwStockInfo.Items.Add(fin.ToString());
            }
        }

        /// <summary>
        /// When stock item in stock listview is selected
        /// update metrics listview with the stocks finanical metrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwAllStocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = lvwAllStocks.SelectedIndex;

            if (index == -1)
                return;

            Stock stock = stockManager.getListItemAt(index);

            lvwStockInfo.Items.Clear();
            lvwStockInfo.Items.Add(stock.ToString());

            foreach(var fin in stock.Financials)
            {
                if(fin != null)
                    lvwStockInfo.Items.Add(fin.ToString());
            }

        }

        /// <summary>
        /// On click, add stock with its name and ticker from textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddStock_Click(object sender, RoutedEventArgs e)
        {
            string name = tbxName.Text;
            string ticker = tbxTicker.Text;
            tbxName.Text = string.Empty;
            tbxTicker.Text = string.Empty;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(ticker))
            {
                MessageBox.Show("Enter name and ticker for a stock.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
                
            Stock stock = stockManager.CreateStock(name, ticker);
            stockManager.addItem(stock); 

            UpdateStockUI();
        }
    }
}
