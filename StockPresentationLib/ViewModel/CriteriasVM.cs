using Microsoft.VisualBasic;
using StockLib.Main.Calculations;
using StockPresentationLib.Utilities;
using StockValuationApp.Entities.Enums;
using StockValuationApp.Entities.Stocks;
using System;
using System.Linq;

namespace StockPresentationLib.ViewModel
{
    public class CriteriasVM : ViewModelBase
    {
        private Stock _stock;

        public CriteriasVM()
        {
        }

        public CriteriasVM(Stock stock)
        {
            Stock = stock;
        }

        public void SetValuationScore(Stock stock)
        {
            stock.StockScore = new StockLib.Main.Entities.Stocks.StockScore();

            if (stock.YearlyFinancials != null && stock.YearlyFinancials.Any())
            {
                double[] revenues = stock.YearlyFinancials
                    .Select(f => f.Revenue)
                    .DefaultIfEmpty(0)
                    .ToArray();

                double[] earnings = stock.YearlyFinancials
                    .Select(f => f.Earnings?.NetIncomeValue ?? 0)
                    .ToArray();

                double[] numOfShares = stock.YearlyFinancials
                    .Select(f => f.NmbrOfShares)
                    .ToArray();

                double[] roes = stock.YearlyFinancials
                    .Select(f => f.KeyFiguresDict != null && f.KeyFiguresDict.ContainsKey(KeyFigureTypes.ReturnOnEquity)
                                 ? (double)f.KeyFiguresDict[KeyFigureTypes.ReturnOnEquity]
                                 : 0)
                    .ToArray();

                double[] roics = stock.YearlyFinancials
                    .Select(f => f.KeyFiguresDict != null && f.KeyFiguresDict.ContainsKey(KeyFigureTypes.ReturnOnInvCap)
                                 ? (double)f.KeyFiguresDict[KeyFigureTypes.ReturnOnInvCap]
                                 : 0)
                    .ToArray();

                var lastYearFinancials = stock.YearlyFinancials.FirstOrDefault(f => f.Year == DateTime.Now.Year - 1 && f.KeyFiguresDict != null);
                double evEbit = 0, evFcf = 0;

                if (lastYearFinancials != null)
                {
                    evEbit = lastYearFinancials.KeyFiguresDict.ContainsKey(KeyFigureTypes.EvEbit)
                        ? (double)lastYearFinancials.KeyFiguresDict[KeyFigureTypes.EvEbit]
                        : 100;

                    evFcf = lastYearFinancials.KeyFiguresDict.ContainsKey(KeyFigureTypes.EvFreecashflow)
                        ? (double)lastYearFinancials.KeyFiguresDict[KeyFigureTypes.EvFreecashflow]
                        : 100;
                }

                stock.StockScore.RevGrowthScore = CalculateStockScore.CalcRevenueGrowthScore(revenues);
                stock.StockScore.EpsGrowthScore = CalculateStockScore.CalcEpsGrowthScore(earnings, numOfShares);
                stock.StockScore.EvFcfScore = CalculateStockScore.CalcEvFcfScore(evFcf);
                stock.StockScore.EvEbitScore = CalculateStockScore.CalcEvEbitScore(evEbit);
                stock.StockScore.RoeRoicScore = CalculateStockScore.CalcRoeRoicScore(roes, roics);
            }
            else
            {
                Console.WriteLine("Financial data is not available.");
            }
        }

        public Stock Stock
        {
            get { return _stock; }
            set {
                _stock = value;
                SetValuationScore(_stock);
                OnPropertyChanged();
            }
        }
    }
}
