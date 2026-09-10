using Microsoft.SemanticKernel;
using StockLib.Main.Agents;
using StockLib.Main.Agents.Analyst;
using StockLib.Main.Agents.Analyst.Technical;
using StockValuationApp.Entities.Stocks;
using System.Collections.ObjectModel;

namespace StockPresentationLib.ViewModel
{
    public class HomeVM : Utilities.ViewModelBase
    {
        private ObservableCollection<Stock> _stocks;
        public EventHandler<Stock> UpdateStockEvent;
        private Stock _currStock;
        private readonly StockManager _stockManager;

        public HomeVM(StockManager stockManager)
        {
            _stockManager = stockManager ?? throw new ArgumentNullException(nameof(stockManager));
            _stocks = new ObservableCollection<Stock>();

            // Hydrate the observable UI collection with our starting objects
            foreach (var stock in _stockManager)
            {
                _stocks.Add(stock);
            }
        }

        public async Task RunFullAgentQueue()
        {
            if (CurrentStock == null || _stockManager == null)
            {
                return;
            }

            await _stockManager.RunAllAgentsSequentiallyAsync(CurrentStock);
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
    }
}
