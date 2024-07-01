using StockPresentationLib.Model;
using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPresentationLib.ViewModel
{
    public class HomeVM : Utilities.ViewModelBase
    {
        private NavigationVM navigationVM;
        public EventHandler<Stock> UpdateStockEvent;

        public HomeVM(NavigationVM navigationVM)
        {
            this.navigationVM = navigationVM;

            if(navigationVM.PersistPageM == null)
                navigationVM.PersistPageM = new PageModel();         
        }

        public Stock GetCurrentStock { get { return navigationVM.PersistPageM.CurrentStock; }}

        public List<Stock> Stocks {
            get
            {
                if (navigationVM.PersistPageM.Stocks != null)
                    return navigationVM.PersistPageM.Stocks;
                else
                {
                    navigationVM.PersistPageM.Stocks = new List<Stock>();   
                    return navigationVM.PersistPageM.Stocks;
                }                
            }
            
            set { 
                if(navigationVM.PersistPageM.Stocks == null)
                    navigationVM.PersistPageM.Stocks = new List<Stock>();
                
                navigationVM.PersistPageM.Stocks = value; 
                OnPropertyChanged(); 
            }  
        }

        public void UpdateCurrentStock(Stock stock)
        {
            UpdateStockEvent?.Invoke(this, stock);
        }
    }
}
