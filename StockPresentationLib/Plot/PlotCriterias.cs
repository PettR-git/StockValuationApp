using ScottPlot.WPF;
using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPresentationLib.Plot
{
    public class PlotCriterias
    {
        private Stock stock;
        public PlotCriterias(Stock stock) 
        {
            this.stock = stock;
        }

        public void PlotValuation(WpfPlot valPlot)
        {
            //Revgrowth Score


            //Earn growth score
            //EV/EBit and EV/FCF score
            //ROE/ROIC score
            //Net debt/ebitda score
        }

        public void PlotMoat(WpfPlot moatPlot)
        {

        }

        public void PlotUnderParam(WpfPlot uParamPlot)
        {

        }
    }
}
