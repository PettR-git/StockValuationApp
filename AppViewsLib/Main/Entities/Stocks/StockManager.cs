using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockValuationApp.Entities.Calculations;
using StockValuationApp.Entities.Enums;
using StockValuationApp.Entities.Stocks.Metrics;
using StockValuationApp.Entities.Stocks.Metrics.Earnings;
using StockValuationApp.Main.Enums;
using StockValuationApp.Main.Uri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WTS.Entities.Main;

namespace StockValuationApp.Entities.Stocks
{
    public class StockManager : ListManager<Stock>
    {
        private UriFinanceManager uriManager;
        private Dictionary<FinanceCategory, List<MetricTypes>> metricTemplate;

        public StockManager()
        {
            uriManager = new UriFinanceManager();
        }

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

        private bool CheckIntsValidity(double[] vals)
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
                    case KeyFigureTypes.ReturnOnInvCap:
                        if (!CheckIntsValidity([e.NetIncome, e.Dividends, e.TotalAssets, e.TotalLiabilities, e.LongTermDebt, e.ShortTermDebt]))
                            continue;

                        keyFigureVal = CalculateKeyFigure.CalcRoic(e.NetIncome, e.Dividends, e.LongTermDebt, e.ShortTermDebt, e.TotalAssets, e.TotalLiabilities);
                        break;

                    case KeyFigureTypes.EvFreecashflow:
                        if (!CheckIntsValidity([ evTuple.ShortTermDebt, evTuple.LongTermDebt, evTuple.MarketValue,
                            evTuple.CashAndEquivalents, e.OperationalCashflow, e.CapitalExpenditures ]))
                        {
                            continue;
                        }

                        keyFigureVal = CalculateKeyFigure.CalcEvFreeCashflow(evTuple, e.OperationalCashflow, e.CapitalExpenditures);
                        break;

                    case KeyFigureTypes.FreeCashflow:
                        if (!CheckIntsValidity([e.OperationalCashflow, e.CapitalExpenditures]))
                            continue;

                        keyFigureVal = e.OperationalCashflow - e.CapitalExpenditures;
                        break;

                    case KeyFigureTypes.ReturnOnEquity:
                        if (!CheckIntsValidity([e.NetIncome, e.TotalAssets, e.TotalLiabilities]))
                            continue;

                        keyFigureVal = CalculateKeyFigure.CalcRoe(e.NetIncome, e.TotalAssets, e.TotalLiabilities);
                        break;

                    case KeyFigureTypes.EvEbitda:

                        if (!CheckIntsValidity(new double[] { evTuple.ShortTermDebt, evTuple.LongTermDebt, evTuple.MarketValue, evTuple.CashAndEquivalents, e.Ebitda }))
                            continue;

                        keyFigureVal = CalculateKeyFigure.CalcEvEarnings(evTuple, e.Ebitda);
                        break;

                    case KeyFigureTypes.EvEbit:

                        if (!CheckIntsValidity(new double[] { evTuple.ShortTermDebt, evTuple.LongTermDebt, evTuple.MarketValue, evTuple.CashAndEquivalents, e.Ebit}))
                            continue;

                        keyFigureVal = CalculateKeyFigure.CalcEvEarnings(evTuple, e.Ebit);
                        break;

                    case KeyFigureTypes.PriceToEarnings:

                        if(!CheckIntsValidity(new double[] {e.NetIncome, e.NumberOfShares, e.Price}))
                            continue;

                        keyFigureVal = CalculateKeyFigure.CalcPriceToEarnings((e.NetIncome, e.NumberOfShares), e.Price);
                        break;

                    case KeyFigureTypes.NetDebtToEbitda:

                        if (!CheckIntsValidity(new double[] { evTuple.ShortTermDebt, evTuple.LongTermDebt, evTuple.CashAndEquivalents, e.Ebitda }))
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

        /// <summary>
        /// Categorize metrics depending on category that the Api is using
        /// </summary>
        private void InitializeMetricTemplate()
        {
            List<FinanceCategory> finCategories = Enum.GetValues(typeof(FinanceCategory)).Cast<FinanceCategory>().ToList();
            List<MetricTypes> metrics = Enum.GetValues(typeof(MetricTypes)).Cast<MetricTypes>().ToList();

            for(int i = 0; i<finCategories.Count(); i++)
            {
                FinanceCategory finCate = finCategories[i];
                
                switch (finCate)
                {
                    case FinanceCategory.Income:
                        metricTemplate.Add(FinanceCategory.Income, new List<MetricTypes> {
                            MetricTypes.netIncome,
                            MetricTypes.ebitda,
                            MetricTypes.revenue,
                            MetricTypes.ebit,
                        });
                        break;
                    case FinanceCategory.BalanceSheet:
                        metricTemplate.Add(FinanceCategory.BalanceSheet, new List<MetricTypes>
                        {
                            MetricTypes.cashAndCashEquivalents,
                            MetricTypes.totalAssets,
                            MetricTypes.totalLiabilities,
                            MetricTypes.longTermDebt,
                            MetricTypes.shortTermDebt,                        
                        });
                        break;
                    case FinanceCategory.Cashflow:
                        metricTemplate.Add(FinanceCategory.Cashflow, new List<MetricTypes>
                        {
                            MetricTypes.operatingCashFlow,
                            MetricTypes.capitalExpenditure,
                            MetricTypes.dividendsPaid,
                        });
                        break;
                    case FinanceCategory.StatementAnalysis:
                        metricTemplate.Add(FinanceCategory.StatementAnalysis, new List<MetricTypes>
                        {
                            MetricTypes.stockPrice,
                            MetricTypes.numberOfShares,
                            MetricTypes.marketCapitalization
                        });
                        break;
                }
            }
        }

        public async void GetSpecificMetricVal(Stock stock)
        {
            List<JObject> jObjs = null;
            MetricEventArgs args = null;
            const int maxApiYearIndex = 5;
            List<int> indexYears = Enumerable.Range(0, maxApiYearIndex).ToList();
            double metricVal = 0.0;

            if (metricTemplate == null)
            {
                metricTemplate = new Dictionary<FinanceCategory, List<MetricTypes>>();
                InitializeMetricTemplate();
            }

            foreach (int i in indexYears)
            {
                args = new MetricEventArgs();
                args.Stock = stock;               

                foreach (var metPair in metricTemplate)
                {
                    FinanceCategory category = metPair.Key;
                    List<MetricTypes> metrics = metPair.Value;

                    foreach (var met in metrics)
                    {
                        switch (category)
                        {
                            case FinanceCategory.Cashflow:
                                jObjs = await uriManager.GetFinanceData(stock.Ticker, FinanceCategory.Cashflow, PeriodTypes.annual);

                                if(jObjs.Count > 0)
                                {
                                    if (!double.TryParse(jObjs[i][met.ToString()]?.ToString(), out metricVal))
                                    {
                                        Console.WriteLine($"Invalid parsing of JObject to double with metric: {met}");
                                        continue;
                                    }                 
                                }
                                else
                                {
                                    Console.WriteLine("No result from API query");
                                    continue;
                                }

                                switch (met)
                                {
                                    case MetricTypes.operatingCashFlow:
                                        args.OperationalCashflow = metricVal;
                                        break;
                                    case MetricTypes.dividendsPaid:
                                        args.Dividends = -metricVal;
                                        break;
                                    case MetricTypes.capitalExpenditure:
                                        args.CapitalExpenditures = -metricVal;
                                        break;
                                }
                                continue;

                            case FinanceCategory.StatementAnalysis:
                                jObjs = await uriManager.GetFinanceData(stock.Ticker, FinanceCategory.StatementAnalysis, PeriodTypes.annual);

                                if (jObjs.Count > 0)
                                {
                                    if (!double.TryParse(jObjs[i][met.ToString()]?.ToString(), out metricVal))
                                    {
                                        Console.WriteLine($"Invalid parsing of JObject to double with metric: {met}");
                                        continue;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No result from API query");
                                    continue;
                                }

                                switch (met)
                                {
                                    case MetricTypes.stockPrice:
                                        args.Price = metricVal;
                                        break;
                                    case MetricTypes.numberOfShares:
                                        args.NumberOfShares = metricVal;
                                        break;
                                    case MetricTypes.marketCapitalization:
                                        args.MarketValue = metricVal;
                                        break;
                                }
                                continue;

                            case FinanceCategory.Income:
                                jObjs = await uriManager.GetFinanceData(stock.Ticker, FinanceCategory.Income, PeriodTypes.annual);

                                if (jObjs.Count > 0)
                                {
                                    if(met == MetricTypes.ebit)
                                    {
                                        bool ebitdaValid = double.TryParse(jObjs[i][MetricTypes.ebitda.ToString()]?.ToString(), out double ebitda);
                                        bool amorAndDepValid = double.TryParse(jObjs[i][MetricTypes.depreciationAndAmortization.ToString()]?.ToString(), out double amortAndDepric);

                                        if (ebitdaValid && amorAndDepValid)
                                        {
                                            metricVal = ebitda - amortAndDepric;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Invalid parsing of JObject to double with metric: {met}");
                                            continue;
                                        }
                                    }
                                    else if (!double.TryParse(jObjs[i][met.ToString()]?.ToString(), out metricVal))
                                    {
                                        Console.WriteLine($"Invalid parsing of JObject to double with metric: {met}");
                                        continue;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No result from API query");
                                    continue;
                                }

                                switch (met)
                                {
                                    case MetricTypes.revenue:
                                        args.Revenue = metricVal;
                                        break;
                                    case MetricTypes.ebitda:
                                        args.Ebitda = metricVal;
                                        break;
                                    case MetricTypes.ebit:
                                        args.Ebit = metricVal;
                                        break;
                                    case MetricTypes.netIncome:
                                        args.NetIncome = metricVal;
                                        break;
                                }
                                continue;

                            case FinanceCategory.BalanceSheet:
                                jObjs = await uriManager.GetFinanceData(stock.Ticker, FinanceCategory.BalanceSheet, PeriodTypes.annual);

                                if (jObjs.Count > 0)
                                {
                                    if (!double.TryParse(jObjs[i][met.ToString()]?.ToString(), out metricVal))
                                    {
                                        Console.WriteLine($"Invalid parsing of JObject to double with metric: {met}");
                                        continue;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No result from API query");
                                    continue;
                                }

                                switch (met)
                                {
                                    case MetricTypes.cashAndCashEquivalents:
                                        args.CashAndEquivalents = metricVal;
                                        break;
                                    case MetricTypes.totalAssets:
                                        args.TotalAssets = metricVal;
                                        break;
                                    case MetricTypes.totalLiabilities:
                                        args.TotalLiabilities = metricVal;
                                        break;
                                    case MetricTypes.longTermDebt:
                                        args.LongTermDebt = metricVal;
                                        break;
                                    case MetricTypes.shortTermDebt:
                                        args.ShortTermDebt = metricVal;
                                        break;
                                }
                                continue;
                        }
                    }                 
                }
                args.Year = DateTime.Now.Year - 1 - i;
                stock.MetricsGiven?.Invoke(this, args);
            }
        }

    }
}
