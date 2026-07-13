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

            // Populate stocks from StockManager (already loaded at app startup)
            foreach (var stock in _stockManager)
            {
                _stocks.Add(stock);
            }
        }

        public StockManager StockManager => _stockManager;

        public Stock GetCurrentStock { get { return _currStock; }}

        public ObservableCollection<Stock> Stocks
        {
            get { return _stocks; }
            set { _stocks = value; OnPropertyChanged(); }
        }

        public void UpdateCurrentStock(Stock stock)
        {
            _currStock = stock;
            UpdateStockEvent?.Invoke(this, stock);
        }
    }
}
