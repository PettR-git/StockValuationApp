using Newtonsoft.Json;
using StockValuationApp.Entities.Calculations;
using StockValuationApp.Entities.Enums;
using StockValuationApp.Entities.Stocks.Metrics;
using StockValuationApp.Entities.Stocks.Metrics.Earnings;
using StockValuationApp.Main.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WTS.Entities.Main;

namespace StockValuationApp.Entities.Stocks
{
    public class StockManager : ListManager<Stock>
    {
        /// <summary>
        /// Create stock object and initialize name and ticker
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ticker"></param>
        /// <returns>stock object</returns>
        public Stock CreateStock(string name, string ticker)
        {
            if (string.IsNullOrEmpty(ticker) || string.IsNullOrEmpty(name))
                return null;

            Stock stock = new Stock();
            stock.Name = name;
            stock.Ticker = ticker;

            return stock;
        }

        /// <summary>
        /// If financials for a specific year exist return true
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="year"></param>
        /// <param name="metricType"></param>
        /// <returns>boolean</returns>
        private (bool, YearlyFinancials) DoesFinanceObjExistFrYear(Stock stock, int year)
        {
            bool yearExist = false;
            YearlyFinancials yearlyFinancials = null;

            foreach (var yf in stock.Financials)
            {
                if (yf.Year == year)
                {
                    yearExist = true;
                    yearlyFinancials = yf;
                }
            }

            return(yearExist, yearlyFinancials);
        }

        private bool CheckIntsValidity(int[] vals)
        {
            foreach (var val in vals)
            {
                if(val == 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Retrieve event args and create financial object
        /// for a stock, given the metric type.
        /// OR if object exist for that year, update it
        /// </summary>
        /// <param name="e">event args from StockInfoWindow</param>
        /// <returns>succesfull creation/update</returns>
        public bool AddMetricDataFrStock(MetricEventArgs e)
        {
            Stock stock = e.Stock;
            int year = e.Year;
            double keyFigureVal = 0.0;

            List<KeyFigureTypes> keyFigureTypes = Enum.GetValues(typeof(KeyFigureTypes)).Cast<KeyFigureTypes>().ToList();
            var evTuple = (e.MarketValue, e.ShortTermDebt, e.LongTermDebt, e.CashAndEquivalents);
            var netDebtTuple = (e.ShortTermDebt, e.LongTermDebt, e.CashAndEquivalents);

            //Calculate specific metric and determine if object creation or update is needed
            (bool yfExist, YearlyFinancials yf) = DoesFinanceObjExistFrYear(stock, year);

            if (yf == null)
            {
                yf = new YearlyFinancials();
            }

            yf.Year = year;
            yf.Earnings = new Earning();
            yf.Earnings.EbitValue = e.Ebit;
            yf.Earnings.EbitdaValue = e.Ebitda;
            yf.Earnings.NetIncomeValue = e.NetIncome;
            yf.Revenue = e.Revenue;
            yf.EnterpriseVal = new EnterpriseValue();
            yf.EnterpriseVal.MarketValue = e.MarketValue;
            yf.EnterpriseVal.LongTermDebt = e.LongTermDebt;
            yf.EnterpriseVal.ShortTermDebt = e.ShortTermDebt;
            yf.EnterpriseVal.CashAndEquivalents = e.CashAndEquivalents;
            yf.NmbrOfShares = e.NumberOfShares;
            yf.StockPrice = e.Price;
            //@TODO Add more stock financials

            if(yfExist == false)
            {
                stock.Financials.Add(yf);
            }

            SortKeyFinancialsByYear(stock);

            foreach (var keyFigureType in keyFigureTypes)
            {
                switch (keyFigureType)
                {
                    case KeyFigureTypes.EvEbitda:

                        if (!CheckIntsValidity(new int[] { evTuple.ShortTermDebt, evTuple.LongTermDebt, evTuple.MarketValue, evTuple.CashAndEquivalents, e.Ebitda }))
                            continue;

                        keyFigureVal = CalculateKeyFigure.CalcEvEarnings(evTuple, e.Ebitda);
                        break;

                    case KeyFigureTypes.EvEbit:

                        if (!CheckIntsValidity(new int[] { evTuple.ShortTermDebt, evTuple.LongTermDebt, evTuple.MarketValue, evTuple.CashAndEquivalents, e.Ebit}))
                            continue;

                        keyFigureVal = CalculateKeyFigure.CalcEvEarnings(evTuple, e.Ebit);
                        break;

                    case KeyFigureTypes.PriceToEarnings:

                        if(!CheckIntsValidity(new int[] {e.NetIncome, e.NumberOfShares, e.Price}))
                            continue;

                        keyFigureVal = CalculateKeyFigure.CalcPriceToEarnings((e.NetIncome, e.NumberOfShares), e.Price);
                        break;

                    case KeyFigureTypes.NetDebtToEbitda:

                        if (!CheckIntsValidity(new int[] { evTuple.ShortTermDebt, evTuple.LongTermDebt, evTuple.CashAndEquivalents, e.Ebitda }))
                            continue;

                        keyFigureVal = CalculateKeyFigure.CalcNetDebtToEbitda(netDebtTuple, e.Ebitda);
                        break;

                    default:
                        Console.WriteLine("Key figure is not yet implemented");
                        continue;
                }

                UpdateOrCreateKeyFigure(stock, yf, keyFigureType, keyFigureVal);
            }

            return true;
        }

        /// <summary>
        /// Update or create keyfigure for stock
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="yf"></param>
        /// <param name="keyFigure"></param>
        /// <param name="result"></param>
        private void UpdateOrCreateKeyFigure(Stock stock, YearlyFinancials yf, KeyFigureTypes keyFigure, double result)
        {
            int index = stock.Financials.IndexOf(yf);

            //if keyfigure dictionary doesnt exist, create dictionary and keyfigure
            if(stock.Financials[index].KeyFiguresDict == null)
            {
                Dictionary<KeyFigureTypes, double> metricDict = new Dictionary<KeyFigureTypes, double>();
                metricDict[keyFigure] = result;

                stock.Financials[index].KeyFiguresDict = metricDict;
            }
            //if keyfigure in keyfigure dictionary doesnt exist, create keyfigure
            else if (!stock.Financials[index].KeyFiguresDict.ContainsKey(keyFigure))
            {
                stock.Financials[index].KeyFiguresDict.Add(keyFigure, result);
            }
            //keyfigure in keyfigure dictionary exist, update it
            else
            {
                stock.Financials[index].KeyFiguresDict[keyFigure] = result;
            }
        }

        public void SortKeyFinancialsByYear(Stock stock)
        {
            stock.Financials = stock.Financials.OrderBy(x => x.Year).ToList();
        }

    }
}
