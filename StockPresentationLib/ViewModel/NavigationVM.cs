using StockPresentationLib.Model;
using StockPresentationLib.Utilities;
using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Input;

namespace StockPresentationLib.ViewModel
{
    public class NavigationVM : ViewModelBase
    {
        private object _currentView;
        public PageModel PersistPageM { get; set; }
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        private void Home(object obj)
        {
            var homeVM = new HomeVM(this);
            homeVM.UpdateStockEvent += OnUpdateCurrentStock;
            CurrentView = homeVM;
        }
        private void Earnings(object obj) => CurrentView = new EarningsVM(PersistPageM.CurrentStock);
        private void Returns(object obj) => CurrentView = new ReturnsVM(PersistPageM.CurrentStock);
        private void Criterias(object obj) => CurrentView = new CriteriasVM();
        private void Consensus(object obj) => CurrentView = new ConsensusVM();
        public ICommand HomeCommand { get; set; }
        public ICommand EarningsCommand { get; set; }
        public ICommand ReturnsCommand { get; set; }
        public ICommand CriteriasCommand { get; set; }
        public ICommand ConsesusCommand { get; set; }
        private void OnUpdateCurrentStock(object sender, Stock stock)
        {
            PersistPageM.CurrentStock = stock;
            OnPropertyChanged(nameof(stock));
        }

        public NavigationVM()
        {
            HomeCommand = new RelayCommand(Home);
            EarningsCommand = new RelayCommand(Earnings);
            ReturnsCommand = new RelayCommand(Returns);
            CriteriasCommand = new RelayCommand(Criterias);
            ConsesusCommand = new RelayCommand(Consensus);
            PersistPageM = new PageModel();

            var homeVM = new HomeVM(this);
            homeVM.UpdateStockEvent += OnUpdateCurrentStock;
            CurrentView = homeVM;
        }
    }
}
