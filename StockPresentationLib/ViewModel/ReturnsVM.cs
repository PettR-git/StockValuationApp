using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPresentationLib.ViewModel
{
    public class ReturnsVM : Utilities.ViewModelBase
    {
        private Stock _stock;
        public ReturnsVM(Stock stock)
        {
            Stock = stock;
        }

        public Stock Stock
        {
            get { return _stock; }
            set { _stock = value; OnPropertyChanged(); }
        }
    }
}
