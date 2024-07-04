using ScottPlot.WPF;
using StockPresentationLib.Plot;
using StockValuationApp.Entities.Stocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPresentationLib.ViewModel
{
    public class EarningsVM : Utilities.ViewModelBase
    {
        private Stock _stock;
        private bool cbxEbitdaG;
        private bool cbxNetIncG;
        private bool cbxRevG;
        private PlotEarnings plotEarn;
        public EarningsVM(Stock stock) 
        { 
            Stock = stock;
        }

        public bool CbxRevGrwthChecked { get { return cbxRevG; } set { cbxRevG = value; OnPropertyChanged(); } }
        public bool CbxEbitGrwthChecked { get { return cbxRevG; } set { cbxRevG = value; OnPropertyChanged(); } }
        public PlotEarnings PlotEarnings { get { return plotEarn; } set { plotEarn = value; OnPropertyChanged(); } }
        public bool CbxEbitdaGrwthChecked { get { return cbxEbitdaG; } set { cbxEbitdaG = value; OnPropertyChanged(); } }
        public bool CbxNetIncGrwthChecked { get { return cbxNetIncG; } set { cbxNetIncG = value; OnPropertyChanged(); } }

        public Stock Stock
        {
            get { return _stock; }
            set { _stock = value; OnPropertyChanged(); }
        }
    }
}
