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
        //public PageModel PersistPageM { get; set; }

        private HomeVM _homeVM;
        private EarningsVM _earningsVM;
        private ReturnsVM _returnsVM;
        private CriteriasVM _criteriasVM;
        private ConsensusVM _consensusVM;

        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        private void Home(object obj)
        {
            if (_homeVM == null)
            {
                _homeVM = new HomeVM();
                _homeVM.UpdateStockEvent += OnUpdateCurrentStock;
            }
            CurrentView = _homeVM;
        }

        private void Earnings(object obj)
        {
            if (_earningsVM == null)
            {
                _earningsVM = new EarningsVM(_currentStock);
            }
            else
            {
                _earningsVM.Stock = _currentStock;
            }
            CurrentView = _earningsVM;
        }

        private void Returns(object obj)
        {
            if (_returnsVM == null)
            {
                _returnsVM = new ReturnsVM(_currentStock);
            }
            else
            {
                _returnsVM.Stock = _currentStock;
            }
            CurrentView = _returnsVM;
        }

        private void Criterias(object obj)
        {
            if (_criteriasVM == null)
            {
                _criteriasVM = new CriteriasVM();
            }
            CurrentView = _criteriasVM;
        }

        private void Consensus(object obj)
        {
            if (_consensusVM == null)
            {
                _consensusVM = new ConsensusVM();
            }
            CurrentView = _consensusVM;
        }

        public ICommand HomeCommand { get; set; }
        public ICommand EarningsCommand { get; set; }
        public ICommand ReturnsCommand { get; set; }
        public ICommand CriteriasCommand { get; set; }
        public ICommand ConsesusCommand { get; set; }

        private void OnUpdateCurrentStock(object sender, Stock stock)
        {
            _currentStock = stock;
            OnPropertyChanged(nameof(stock));
        }

        public NavigationVM()
        {
            HomeCommand = new RelayCommand(Home);
            EarningsCommand = new RelayCommand(Earnings);
            ReturnsCommand = new RelayCommand(Returns);
            CriteriasCommand = new RelayCommand(Criterias);
            ConsesusCommand = new RelayCommand(Consensus);

            _homeVM = new HomeVM();
            _homeVM.UpdateStockEvent += OnUpdateCurrentStock;
            CurrentView = _homeVM;
        }
    }

}
