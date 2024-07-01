using ScottPlot.WPF;
using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPresentationLib.Model
{
    public class PageModel
    {
        //Home (High persistance)
        public List<Stock> Stocks { get; set; }
        public Stock CurrentStock { get; set; }

        //Earnings
        public WpfPlot FinPlot { get; set; }
        public bool EbitdaGrwthChecked {  get; set; }
        public bool NetIncGrwthChecked { get; set; }
        public bool RevenueGrwthChecked { get; set; }

    }
}
