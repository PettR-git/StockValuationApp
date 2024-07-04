using StockPresentationLib.Plot;
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
        private bool cbxRoeG;
        private bool cbxRoicG;
        private bool cbxFcfG;
        private bool firstPlot;
        public ReturnsVM(Stock stock)
        {
            Stock = stock;
            firstPlot = true;
        }

        public bool FirstPlot { get { return firstPlot; } set { firstPlot = value; OnPropertyChanged(); } }
        public bool CbxFcfGrwthChecked { get { return cbxFcfG; } set { cbxFcfG = value; OnPropertyChanged(); } }
        public bool CbxRoeGrwthChecked { get { return cbxRoeG; } set { cbxRoeG = value; OnPropertyChanged(); } }
        public bool CbxRoicGrwthChecked { get { return cbxRoicG; } set { cbxRoicG = value; OnPropertyChanged(); } }


        public Stock Stock
        {
            get { return _stock; }
            set { _stock = value; OnPropertyChanged(); }
        }
    }
}
