using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPresentationLib.ViewModel
{
    public class HomeVM : Utilities.ViewModelBase
    {
        private ObservableCollection<Stock> stocks;
        public EventHandler<Stock> UpdateStockEvent;
        private Stock currStock;

        public HomeVM()
        {
            Stocks = new ObservableCollection<Stock>();       
        }

        public Stock GetCurrentStock { get { return currStock; }}

        public ObservableCollection<Stock> Stocks
        {
            get{ return stocks;}
            set { stocks = value; OnPropertyChanged();}
        }

        public void UpdateCurrentStock(Stock stock)
        {
            currStock = stock;
            UpdateStockEvent?.Invoke(this, stock);
        }
    }
}
