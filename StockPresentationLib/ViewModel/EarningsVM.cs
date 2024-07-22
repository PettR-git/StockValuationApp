using ScottPlot.WPF;
using StockPresentationLib.Plot;
using StockValuationApp.Entities.Enums;
using StockValuationApp.Entities.Stocks;
using StockValuationApp.Entities.Stocks.Metrics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<YearlyFinancials> yearlyFinancials;
        private decimal ebitdaMargin;
        private decimal ebitMargin;
        private decimal netIncMargin;
        private Dictionary<int, Dictionary<KeyFigureTypes, double>> keyFigureDict;

        public EarningsVM(Stock stock) 
        { 
            Stock = stock;
            YearlyFinancials = new ObservableCollection<YearlyFinancials>();
            keyFigureDict = new Dictionary<int, Dictionary<KeyFigureTypes, double>>();
        }

        public Stock Stock
        {
            get { return _stock; }
            set { _stock = value; OnPropertyChanged(); }
        }

        //Plot
        public bool CbxRevGrwthChecked { get { return cbxRevG; } set { cbxRevG = value; OnPropertyChanged(); } }
        public bool CbxEbitGrwthChecked { get { return cbxRevG; } set { cbxRevG = value; OnPropertyChanged(); } }
        public PlotEarnings PrevPlotEarnings { get { return plotEarn; } set { plotEarn = value; OnPropertyChanged(); } }
        public bool CbxEbitdaGrwthChecked { get { return cbxEbitdaG; } set { cbxEbitdaG = value; OnPropertyChanged(); } }
        public bool CbxNetIncGrwthChecked { get { return cbxNetIncG; } set { cbxNetIncG = value; OnPropertyChanged(); } }

        //Earnings
        public ObservableCollection<YearlyFinancials> YearlyFinancials
        {
            get { return yearlyFinancials; }
            set 
            { 
                if(value != null)
                {
                    yearlyFinancials = value;
                }
            }
        }

        public Dictionary<int, Dictionary<KeyFigureTypes, double>> KeyFiguresDict
        {
            get { return keyFigureDict; }
            set { keyFigureDict = value; OnPropertyChanged(); }
        }

        public decimal EbitdaMargin { get {  return ebitdaMargin; } set {  ebitdaMargin = value; OnPropertyChanged(); }}
        public decimal EbitMargin {  get { return ebitMargin; } set {  ebitMargin = value; OnPropertyChanged(); } }
        public decimal NetIncMargin { get { return netIncMargin; } set { netIncMargin = value; OnPropertyChanged(); } }
    }
}
