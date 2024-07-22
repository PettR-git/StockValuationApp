using Microsoft.Win32;
using StockValuationApp.Entities.Stocks.Metrics;
using StockValuationApp.Entities.Stocks;
using StockValuationApp.Main.Utilities;
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
using System.Xml.Linq;
using StockPresentationLib.ViewModel;

namespace StockPresentationLib.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        private StockManager stockManager;
        private StockInfoWindow stockInfoWindow;
        private StockPlotWindow stockPlotWindow;
        private string filename;
        private StockJsonSerializerSettings jsonSerializerSettings;
        private HomeVM homeVM;
        public Home()
        {
            InitializeComponent();
            stockManager = new StockManager();
            jsonSerializerSettings = new StockJsonSerializerSettings();
            //this.Loaded += Home_Loaded;
            //this.Unloaded += Home_Unloaded;
            this.DataContextChanged += Home_DataContextChanged;
        }

        private void Home_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            InitializeGUI();

            if (DataContext is HomeVM homeDataContext)
            {
                homeVM = homeDataContext;
                List<Stock> stocks = homeVM.Stocks.ToList();

                if (stocks.Count > 0)
                {
                    int index = 0;

                    foreach (Stock stock in stocks)
                    {
                        if (stock == homeVM.GetCurrentStock)
                        {
                            index = stocks.IndexOf(stock);
                        }

                        stockManager.addItem(stock);
                    }
                    lvwAllStocks.SelectedIndex = index;
                }
            }
        }

        //Initialize UI
        private void InitializeGUI()
        {
            string metricStr = string.Empty;
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

            if (index == -1)
                return;

            Stock stock = stockManager.getListItemAt(index);

            if (stock != null)
            {
                stockInfoWindow = new StockInfoWindow(stock);
                stock.MetricsGiven += OnGetMetricsData;
                stockInfoWindow.ShowDialog();

                UpdateFinancialUI(stock);
            }
        }

        /// <summary>
        /// Method as a subscriber to metric event
        /// To add metricsdata from event args
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">holds metric data for stock</param>
        private void OnGetMetricsData(object sender, MetricEventArgs e)
        {
            Stock stock = e.Stock;

            if (!stockManager.AddMetricDataFrStock(e))
            {
                MessageBox.Show("Error in adding metrics data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UpdateFinancialUI(stock);
        }

        private void NewApp()
        {
            stockManager = new StockManager();
            lvwStockInfo.Items.Clear();
            //UpdateStockUI();
        }

        /// <summary>
        /// Update the stocks listview
        /// </summary>
        private void UpdateStockUI()
        {
            Stock stock = null;
            lvwAllStocks.Items.Clear();

            for (int i = 0; i < stockManager.Count(); i++)
            {
                stock = stockManager.getListItemAt(i);

                ListViewItem item = new ListViewItem();
                item.Content = stock.ToString();
                item.FontFamily = lvwAllStocks.FontFamily;

                lvwAllStocks.Items.Add(item);
            }
        }

        /// <summary>
        /// Update metricsListview for a stocks metric data
        /// </summary>
        /// <param name="stock"></param>
        private void UpdateFinancialUI(Stock stock)
        {
            lvwStockInfo.Items.Clear();

            foreach (var fin in stock.Financials)
            {
                if (fin != null)
                {
                    ListViewItem item = new ListViewItem();
                    item.Content = fin.ToString();
                    item.FontFamily = lvwStockInfo.FontFamily;

                    lvwStockInfo.Items.Add(item);
                }
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

            homeVM.UpdateCurrentStock(stock);

            lvwStockInfo.Items.Clear();
            tbkKeyFinancialFigures.Text = $"Key Financial Figures for {stock.Name}";

            foreach (var fin in stock.Financials)
            {
                if (fin != null)
                {
                    lvwStockInfo.Items.Add(fin.ToString());
                }
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

            //UpdateStockUI();
            homeVM.Stocks.Add(stock);
        }

        private void btnGraphs_Click(object sender, RoutedEventArgs e)
        {
            int index = lvwAllStocks.SelectedIndex;

            if (index == -1)
                return;

            List<YearlyFinancials> yearlyFinancials = stockManager.getListItemAt(index).Financials;

            if (yearlyFinancials != null || yearlyFinancials.Count <= 0)
            {
                stockPlotWindow = new StockPlotWindow(yearlyFinancials);
                stockPlotWindow.Show();
            }
        }

        private void btnDeleteStock_Click(object sender, RoutedEventArgs e)
        {
            int index = lvwAllStocks.SelectedIndex;

            if (index == -1) return;

            Stock stock = stockManager.getListItemAt(index);

            if (stock != null)
            {
                stockManager.removeItem(stock);
                homeVM.Stocks.Remove(stock);
            }

            //UpdateStockUI();
        }

        private void btnDeleteYearlyFin_Click(object sender, RoutedEventArgs e)
        {
            int yfIndex = lvwStockInfo.SelectedIndex;
            int stockIndex = lvwAllStocks.SelectedIndex;

            if (yfIndex == -1 || stockIndex == -1) return;

            Stock stock = stockManager.getListItemAt(stockIndex);
            YearlyFinancials yf = stock.Financials.ElementAt(yfIndex);

            if (yf != null)
            {
                stock.Financials.Remove(yf);
            }

            UpdateFinancialUI(stock);
        }

        private void btnImportStockData_Click(object sender, RoutedEventArgs e)
        {
            int index = lvwAllStocks.SelectedIndex;

            if (index == -1) return;
            Stock stock = stockManager.getListItemAt(index);

            if (stock != null)
            {
                try
                {
                    stock.MetricsGiven += OnGetMetricsData;

                    stockManager.GetSpecificMetricVal(stock);

                    UpdateFinancialUI(stock);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in importing stock data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        #region File Handling
        //In case file action was not wanted
        private bool continueWithFileAction()
        {
            bool ready = false;
            MessageBoxResult res = MessageBox.Show("Are you sure you want to proceed?", "Confirmation", MessageBoxButton.YesNo);

            if (res == MessageBoxResult.Yes)
                ready = true;

            return ready;
        }

        /// <summary>
        /// New file creation, essentially creating a new instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newFile_click(object sender, RoutedEventArgs e)
        {
            if (continueWithFileAction())
                NewApp();
        }

        /// <summary>
        /// Open text file, with binary serialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileOpenTF_click(object sender, RoutedEventArgs e)
        {
            if (!continueWithFileAction())
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    filename = openFileDialog.FileName;
                    NewApp();

                    if (!stockManager.binaryDeSerialize(filename))
                        MessageBox.Show("No data provided from file.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in file handling!");
                    Console.WriteLine(ex.Message);
                    return;
                }

                UpdateStockUI();
            }
        }


        /// <summary>
        /// Open json file, with serializersettings from util
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileOpenJson_click(object sender, RoutedEventArgs e)
        {
            if (!continueWithFileAction())
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files (*.json)|*.json";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    filename = openFileDialog.FileName;
                    NewApp();

                    if (!stockManager.jsonDeSerialize(filename, jsonSerializerSettings.JsonSettings))
                        MessageBox.Show("Could not import data");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in file handling!");
                    Console.WriteLine(ex.Message);
                    return;
                }

                UpdateStockUI();
            }
        }

        /// <summary>
        /// Save current file as current file type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileSave_click(object sender, RoutedEventArgs e)
        {
            if (!continueWithFileAction())
                return;
            else if (filename == null)
            {
                MessageBox.Show("Save as a new file", "Error");
            }
            else if (filename.Substring(filename.Length - 4) == "json")
            {
                try
                {
                    stockManager.jsonSerialize(filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in writing data to xml file!");
                    Console.WriteLine(ex.Message);
                }
            }
            else if (filename.Substring(filename.Length - 3) == "txt")
            {
                try
                {
                    stockManager.binarySerialize(filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error writing data to text file!");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Save as textfile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileSaveAsTF_click(object sender, RoutedEventArgs e)
        {
            if (!continueWithFileAction())
                return;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    filename = saveFileDialog.FileName;
                    stockManager.binarySerialize(filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in writing data to file!");
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Save as json with its serializer settings from util
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFileSaveAsJson_click(object sender, RoutedEventArgs e)
        {
            if (!continueWithFileAction())
                return;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files (*.json)|*.json";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    filename = saveFileDialog.FileName;
                    stockManager.jsonSerialize(filename, jsonSerializerSettings.JsonSettings);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in writing data to file!");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        //Exit the app from menu
        private void mnuFileExportExit_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion
    }
}
