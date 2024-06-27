using ScottPlot.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPresentationLib.Model
{
    public class PageModel
    {
        //Home

        //Earnings
        public WpfPlot FinPlot { get; set; }
        public bool EbitdaGrwthChecked {  get; set; }
        public bool NetIncGrwthChecked { get; set; }
        public bool RevenueGrwthChecked { get; set; }

    }
}
