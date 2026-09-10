using Microsoft.Win32;
using StockLib.Main.Entities.Stocks.Metrics;
using StockPresentationLib.ViewModel;
using StockValuationApp.Entities.Stocks;
using StockValuationApp.Entities.Stocks.Metrics;
using StockValuationApp.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Xml.Linq;

namespace StockPresentationLib.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        private StockManager _stockManager; 
        private HomeVM _homeVM;
        private StockInfoWindow _stockInfoWindow;
        private StockPlotWindow _stockPlotWindow;
        private string _filename;
        private StockJsonSerializerSettings? _jsonSerializerSettings;

        public Home()
        {
            InitializeComponent();
            _jsonSerializerSettings = new StockJsonSerializerSettings();

            this.Loaded += Home_Loaded;
            this.DataContextChanged += Home_DataContextChanged;
        }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is HomeVM vm)
            {
                _homeVM = vm;
                _stockManager = vm.StockManager;

                // Preserve current stock ticker before Sync clears the list
                string? activeTicker = _homeVM.CurrentStock?.Ticker;

                SyncStocksToViewModel();

                // Restore active stock after repopulating Stocks
                if (!string.IsNullOrEmpty(activeTicker))
                {
                    var matchedStock = _homeVM.Stocks.FirstOrDefault(s => s.Ticker == activeTicker);
                    if (matchedStock != null)
                    {
                        // Assign to VM and UI using Dispatcher so layout container generation completes
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _homeVM.CurrentStock = matchedStock;
                            lvwAllStocks.SelectedItem = matchedStock;
                            lvwAllStocks.ScrollIntoView(matchedStock);
                            UpdateStockDetailsUI(matchedStock);
                        }), System.Windows.Threading.DispatcherPriority.Loaded);
                    }
                }
            }
        }

        private void Home_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            InitializeGUI();

            if (DataContext is HomeVM homeDataContext)
            {
                _homeVM = homeDataContext;
                _stockManager = _homeVM.StockManager;
                SyncStocksToViewModel();
            }
        }

        // Initialize UI
        private void InitializeGUI()
        {
            string metricStr = string.Empty;
        }

        /// <summary>
        /// Retrieve index from selected item in listview, get corresponding stock object
        /// And visualize all metrics data for the object
        /// </summary>
        private async void btnCalculateValuation_Click(object sender, RoutedEventArgs e)
        {
            int index = lvwAllStocks.SelectedIndex;

            if (index == -1)
                return;

            Stock stock = _stockManager.getListItemAt(index);

            if (stock != null)
            {
                _stockInfoWindow = new StockInfoWindow(stock);
                stock.MetricsGiven += OnGetMetricsData;
                _stockInfoWindow.ShowDialog();

                UpdateFinancialUI(stock);
                await SaveStocksAsync();
            }
        }

        /// <summary>
        /// Method as a subscriber to metric event
        /// To add metricsdata from event args
        /// </summary>
        private async void OnGetMetricsData(object sender, MetricEventArgs e)
        {
            Stock? stock = e.Stock;

            if (!_stockManager.AddMetricDataFrStock(e))
            {
                MessageBox.Show("Error in adding metrics data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (stock != null)
            {
                UpdateFinancialUI(stock);
                await SaveStocksAsync();
            }
        }

        private async void OnGetWeeklyStockPricesData(object sender, WeeklyStockPricesEventArgs e)
        {
            if (!_stockManager.AddWeeklyPriceDataFrStock(e))
            {
                MessageBox.Show("Error in adding price data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await SaveStocksAsync();         
        }

        /// <summary>
        /// Sync the ViewModel collection with StockManager data
        /// </summary>
        private void SyncStocksToViewModel()
        {
            if (_homeVM == null || _stockManager == null)
            {
                return;
            }

            _homeVM.Stocks.Clear();
            foreach (var stock in _stockManager)
            {
                _homeVM.Stocks.Add(stock);
            }
        }

        /// <summary>
        /// Update metricsListview for a stocks metric data
        /// </summary>
        private void UpdateFinancialUI(Stock stock)
        {
            lvwStockInfo.Items.Clear();

            foreach (var fin in stock.YearlyFinancials)
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
        private void lvwAllStocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvwAllStocks.SelectedItem is Stock selectedStock)
            {
                _homeVM.CurrentStock = selectedStock;
                UpdateStockDetailsUI(selectedStock);
            }
        }

        private void UpdateStockDetailsUI(Stock stock)
        {
            lvwStockInfo.Items.Clear();
            tbkKeyFinancialFigures.Text = $"Key Financial Figures for {stock.Name}";

            if (stock.YearlyFinancials != null)
            {
                foreach (var fin in stock.YearlyFinancials)
                {
                    if (fin != null)
                    {
                        lvwStockInfo.Items.Add(fin.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// On click, add stock with its name and ticker from textboxes
        /// </summary>
        private async void btnAddStock_Click(object sender, RoutedEventArgs e)
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

            Stock? stock = _stockManager.CreateStock(name, ticker);
            if (stock != null)
            {
                _stockManager.addItem(stock);
                _homeVM.Stocks.Add(stock);
                await SaveStocksAsync();
            }
        }

        private void btnGraphs_Click(object sender, RoutedEventArgs e)
        {
            int index = lvwAllStocks.SelectedIndex;

            if (index == -1)
                return;

            List<YearlyFinancials> yearlyFinancials = _stockManager.getListItemAt(index).YearlyFinancials;

            if (yearlyFinancials != null && yearlyFinancials.Count > 0)
            {
                _stockPlotWindow = new StockPlotWindow(yearlyFinancials);
                _stockPlotWindow.Show();
            }
        }

        private async void btnDeleteStock_Click(object sender, RoutedEventArgs e)
        {
            int index = lvwAllStocks.SelectedIndex;

            if (index == -1) return;

            Stock stock = _stockManager.getListItemAt(index);

            if (stock != null)
            {
                _stockManager.removeItem(stock);
                _homeVM.Stocks.Remove(stock);
                await SaveStocksAsync();
            }
        }

        private async void btnDeleteYearlyFin_Click(object sender, RoutedEventArgs e)
        {
            int yfIndex = lvwStockInfo.SelectedIndex;
            int stockIndex = lvwAllStocks.SelectedIndex;

            if (yfIndex == -1 || stockIndex == -1) return;

            Stock stock = _stockManager.getListItemAt(stockIndex);
            YearlyFinancials yf = stock.YearlyFinancials.ElementAt(yfIndex);

            if (yf != null)
            {
                stock.YearlyFinancials.Remove(yf);
                await SaveStocksAsync();
            }

            UpdateFinancialUI(stock);
        }

        private async void btnImportStockData_Click(object sender, RoutedEventArgs e)
        {
            int index = lvwAllStocks.SelectedIndex;

            if (index == -1) return;
            Stock stock = _stockManager.getListItemAt(index);

            if (stock != null)
            {
                try
                {
                    stock.MetricsGiven += OnGetMetricsData;
                    stock.WeeklyStockPricesGiven += OnGetWeeklyStockPricesData;
                    await _stockManager.GetMetricVals(stock);
                    UpdateFinancialUI(stock);
                    await SaveStocksAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in importing stock data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private async Task SaveStocksAsync()
        {
            if (_stockManager == null)
            {
                return;
            }

            await _stockManager.SaveAllAsync().ConfigureAwait(false);
        }

        private async void btnAiAnalysis_Click(object sender, RoutedEventArgs e)
        {
            btnAiAnalysis.IsEnabled = false;

            try
            {
                await _homeVM.RunFullAgentQueue();
                await SaveStocksAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UI Error] Analysis failed: {ex.Message}");

                MessageBox.Show(
                    $"Failed to run AI analysis:\n\n{ex.Message}",
                    "Analysis Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                btnAiAnalysis.IsEnabled = true;
            }
        }

        #region File Handling
        private bool continueWithFileAction()
        {
            bool ready = false;
            MessageBoxResult res = MessageBox.Show("Are you sure you want to proceed?", "Confirmation", MessageBoxButton.YesNo);

            if (res == MessageBoxResult.Yes)
                ready = true;

            return ready;
        }

        private void newFile_click(object sender, RoutedEventArgs e)
        {
            if (continueWithFileAction())
            {
                lvwStockInfo.Items.Clear();
            }
        }

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
                    _filename = openFileDialog.FileName;

                    if (!_stockManager.binaryDeSerialize(_filename))
                        MessageBox.Show("No data provided from file.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in file handling!");
                    Console.WriteLine(ex.Message);
                    return;
                }

                SyncStocksToViewModel();
            }
        }

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
                    _filename = openFileDialog.FileName;
                    var jsonSettings = _jsonSerializerSettings?.AddJsonSerializerSettings();

                    if (!_stockManager.jsonDeSerialize(_filename, jsonSettings))
                        MessageBox.Show("No data provided from file.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in file handling!");
                    Console.WriteLine(ex.Message);
                    return;
                }

                SyncStocksToViewModel();
            }
        }

        private void mnuFileSave_click(object sender, RoutedEventArgs e)
        {
            // Save via persistence layer
        }

        private void mnuFileSaveAsTF_click(object sender, RoutedEventArgs e)
        {
            // Custom export to text file
        }

        private void mnuFileSaveAsJson_click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files (*.json)|*.json";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _filename = saveFileDialog.FileName;
                    // Export as JSON if needed
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in file handling!");
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
        }

        private void mnuFileExportExit_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

    }
}
