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
        private Stock _currentStock;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand HomeCommand { get; set; }
        public ICommand EarningsCommand { get; set; }
        public ICommand ReturnsCommand { get; set; }
        public ICommand CriteriasCommand { get; set; }
        public ICommand ConsesusCommand { get; set; }

        private void Home(object obj) => CurrentView = new HomeVM();
        private void Earnings(object obj) => CurrentView = new EarningsVM();
        private void Returns(object obj) => CurrentView = new ReturnsVM();
        private void Criterias(object obj) => CurrentView = new CriteriasVM();
        private void Consensus(object obj) => CurrentView = new ConsensusVM();

        public NavigationVM()
        {
            HomeCommand = new RelayCommand(Home);
            EarningsCommand = new RelayCommand(Earnings);
            ReturnsCommand = new RelayCommand(Returns);
            CriteriasCommand = new RelayCommand(Criterias);
            ConsesusCommand = new RelayCommand(Consensus);

            CurrentView = new HomeVM();
        }
    }
}
}
