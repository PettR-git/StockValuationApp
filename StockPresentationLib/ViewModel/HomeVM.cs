using StockLib.Main.Agents.Analyst;
using StockValuationApp.Entities.Stocks;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace StockPresentationLib.ViewModel
{
    public class HomeVM : Utilities.ViewModelBase
    {
        private ObservableCollection<Stock> _stocks;
        public EventHandler<Stock> UpdateStockEvent;
        private Stock _currStock;
        private readonly StockManager _stockManager;
        private bool _isAgentAnalyzing;
        private readonly StockAnalysisOverviewAgent _analystAgent;

        public HomeVM(StockManager stockManager, StockAnalysisOverviewAgent analystAgent)
        {
            _stockManager = stockManager ?? throw new ArgumentNullException(nameof(stockManager));
            _analystAgent = analystAgent ?? throw new ArgumentNullException(nameof(analystAgent));
            _stocks = new ObservableCollection<Stock>();

            // Hydrate the observable UI collection with our starting objects
            foreach (var stock in _stockManager)
            {
                _stocks.Add(stock);
            }
        }

        public bool IsAgentAnalyzing
        {
            get => _isAgentAnalyzing;
            set { _isAgentAnalyzing = value; OnPropertyChanged(); }
        }


        public StockManager StockManager => _stockManager;

        public Stock CurrentStock
        {
            get => _currStock;
            set
            {
                if (_currStock != value)
                {
                    _currStock = value;
                    OnPropertyChanged(); // Uses [CallerMemberName], notifying "CurrentStock" automatically
                    UpdateStockEvent?.Invoke(this, _currStock);
                }
            }
        }

        public ObservableCollection<Stock> Stocks
        {
            get { return _stocks; }
            set { _stocks = value; OnPropertyChanged(); }
        }


        // Change signature from 'public async void' or 'public void' to 'public async Task'
        public async Task AnalyzeStockWithAgentAsync()
        {
            if (_currStock == null)
            {
                Debug.WriteLine("[HomeVM] No stock selected for analysis.");
                return;
            }

            // Call your agent
            await RunAgentAnalysisAsync(_currStock);
        }

        private async Task RunAgentAnalysisAsync(Stock stock)
        {
            IsAgentAnalyzing = true;

            try
            {
                // Run completely off the main UI thread to ensure zero stuttering
                string analysisResult = await Task.Run(async () =>
                    await _analystAgent.GenerateContextualAnalysisAsync(stock)
                );

                // This triggers the PropertyChanged implementation directly inside the Stock instance.
                // WPF automatically intercepts this cross-thread property change and refreshes the XAML View safely.
                stock.AgentAnalysisJson = analysisResult;
            }
            catch (Exception ex)
            {
                stock.AgentAnalysisJson = $"Failed to generate context: {ex.Message}";
            }
            finally
            {
                IsAgentAnalyzing = false;
            }
        }
    }
}
