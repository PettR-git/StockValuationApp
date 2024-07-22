using StockPresentationLib.Utilities;
using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPresentationLib.ViewModel
{
    public class CriteriasVM : ViewModelBase
    {
        private Stock _stock;

        public CriteriasVM(Stock stock)
        {
            _stock = stock;
        }
        public Stock Stock
        {
            get { return _stock; }
            set { _stock = value; OnPropertyChanged(); }
        }
    }
}
