using StockPresentationLib.Utilities;
using StockValuationApp.Entities.Stocks;
using System;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace StockPresentationLib.ViewModel
{
    public class NavigationVM : ViewModelBase
    {
        private readonly IServiceProvider _provider;
        private object _currentView;
        private Stock _currentStock;

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
                _homeVM = _provider.GetRequiredService<HomeVM>();
                _homeVM.UpdateStockEvent += OnUpdateCurrentStock;
            }
            CurrentView = _homeVM;
        }

        private void Earnings(object obj)
        {
            if (_earningsVM == null)
            {
                _earningsVM = _provider.GetRequiredService<EarningsVM>();
            }

            _earningsVM.Stock = _currentStock;
            CurrentView = _earningsVM;
        }

        private void Returns(object obj)
        {
            if (_returnsVM == null)
            {
                _returnsVM = _provider.GetRequiredService<ReturnsVM>();
            }

            _returnsVM.Stock = _currentStock;
            CurrentView = _returnsVM;
        }

        private void Criterias(object obj)
        {
            if (_criteriasVM == null)
            {
                _criteriasVM = _provider.GetRequiredService<CriteriasVM>();
            }

            if (_criteriasVM.Stock != _currentStock)
            {
                _criteriasVM.Stock = _currentStock;
            }
            CurrentView = _criteriasVM;
        }

        private void Consensus(object obj)
        {
            if (_consensusVM == null)
            {
                _consensusVM = _provider.GetRequiredService<ConsensusVM>();
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

        // DI-friendly constructor: accept IServiceProvider to resolve transient VMs on demand
        public NavigationVM(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));

            HomeCommand = new RelayCommand(Home);
            EarningsCommand = new RelayCommand(Earnings);
            ReturnsCommand = new RelayCommand(Returns);
            CriteriasCommand = new RelayCommand(Criterias);
            ConsesusCommand = new RelayCommand(Consensus);

            // Initialize home VM from the provider and subscribe to its update event
            _homeVM = _provider.GetRequiredService<HomeVM>();
            _homeVM.UpdateStockEvent += OnUpdateCurrentStock;
            CurrentView = _homeVM;
        }
    }
}
