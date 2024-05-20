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

        private void InitializeGUI()
        {
            MetricType[] metricTypes = (MetricType[])Enum.GetValues(typeof(MetricType));
            foreach(MetricType metricType in metricTypes)
            {
                cmbMetrics.Items.Add(metricType);
            }
        }

        private void btnCalculateValuation_Click(object sender, RoutedEventArgs e)
        {
            int index = lvwAllStocks.SelectedIndex;
            Stock stock = stockManager.getListItemAt(index);

            if (index == -1 || stock == null || cmbMetrics.SelectedItem == null)
                return;

            MetricType metricType = (MetricType)cmbMetrics.SelectedItem;
            stockInfoWindow = new StockInfoWindow(metricType, stock);
            stockInfoWindow.MetricsGiven += OnGetMetricsData;
            stockInfoWindow.ShowDialog();

            UpdateFinancialUI(stock);
        }

        private void OnGetMetricsData(object sender, MetricEventArgs e)
        {
            int index = lvwAllStocks.SelectedIndex;

            if (!stockManager.AddMetricDataFrStock(e))
                MessageBox.Show("Error in adding metrics data");
        }

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

        private void UpdateFinancialUI(Stock stock)
        {;
            lvwStockInfo.Items.Clear();
            lvwStockInfo.Items.Add(stock.ToString());

            foreach (var fin in stock.Financials)
            {
                if (fin != null)
                    lvwStockInfo.Items.Add(fin.ToString());
            }
        }

        private void lvwAllStocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = lvwAllStocks.SelectedIndex;

            if (index == -1)
            {
                MessageBox.Show("Error in retrieving stock from listview");
                return;
            }

            Stock stock = stockManager.getListItemAt(index);

            lvwStockInfo.Items.Clear();
            lvwStockInfo.Items.Add(stock.ToString());

            foreach(var fin in stock.Financials)
            {
                if(fin != null)
                    lvwStockInfo.Items.Add(fin.ToString());
            }

        }

        private void btnAddStock_Click(object sender, RoutedEventArgs e)
        {
            string name = tbxName.Text;
            string ticker = tbxTicker.Text;

            Stock stock = stockManager.CreateStock(name, ticker);
            stockManager.addItem(stock);
        }
    }
}
