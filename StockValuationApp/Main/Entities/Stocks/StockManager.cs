using Newtonsoft.Json;
using StockValuationApp.Entities.Calculations;
using StockValuationApp.Entities.Enums;
using StockValuationApp.Entities.Stocks.Metrics;
using StockValuationApp.Entities.Stocks.Metrics.Earnings;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private bool DoesFinanceObjExistFrYear(Stock stock, int year, MetricType metricType)
        {
            if(stock.Financials != null)
            {
                foreach (var yf in stock.Financials)
                {
                    if (yf.Year == year)
                    {
                        return true;
                    }
                }
            }

            return false;
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
            MetricType metricType = e.MetricType;
            Stock stock = e.Stock;
            int year = e.Year;
            double metricVal = 0.0;

            //Calculate specific metric and determine if object creation or update is needed
            switch (metricType)
            {
                case MetricType.EvEbitda:

                    metricVal = CalculateValuationMetric.CalcEvEarnings((e.MarketValue, e.NetDebt), e.Ebitda);
                    if (!DoesFinanceObjExistFrYear(stock, year, metricType))
                    {
                        CreateEvEbitdaMetric(e, metricVal);
                        return true;
                    }
                    break;

                case MetricType.EvEbit:
                    metricVal = CalculateValuationMetric.CalcEvEarnings((e.MarketValue, e.NetDebt), e.Ebit);
                    if (!DoesFinanceObjExistFrYear(stock, year, metricType))
                    {
                        CreateEvEbitMetric(e, metricVal);
                        return true;
                    }
                    break;

                case MetricType.PriceToEarnings:
                    metricVal = CalculateValuationMetric.CalcPriceToEarnings((e.NetIncome, e.NumberOfShares), e.Price);
                    if (!DoesFinanceObjExistFrYear(stock, year, metricType))
                    {
                        CreatePriceToEarningsMetric(e, metricVal);
                        return true;
                    }                 
                    break;

                case MetricType.NetDebtToEbitda:
                    metricVal = CalculateValuationMetric.CalcNetDebtToEbitda(e.NetDebt, e.Ebitda);
                    if (!DoesFinanceObjExistFrYear(stock, year, metricType))
                    {
                        CreateNetDebtToEbitdaMetric(e, metricVal);
                        return true;
                    }
                    break;

                default:
                    Console.WriteLine("Metrictype is null or invalid value");
                    return false;
            }

            //In case finance object exist for that year, update finance obj and its metric data
            foreach (var yf in stock.Financials)
            {
                if(yf.Year == year)
                {
                    if (yf.MetricDict.ContainsKey(metricType))
                    {
                        yf.MetricDict[metricType] = metricVal;
                    }
                    else
                    {
                        yf.MetricDict.Add(metricType, metricVal);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Create price to earnings metric and intantiatiate its properties
        /// </summary>
        /// <param name="e"></param>
        /// <param name="result"></param>
        private void CreatePriceToEarningsMetric(MetricEventArgs e, double result)
        {
            Earning earn = new NetIncome();
            earn.NetIncomeValue = e.NetIncome;

            Dictionary<MetricType, double> metricDict = new Dictionary<MetricType, double>();
            metricDict[MetricType.PriceToEarnings] = result;

            e.Stock.Financials.Add(new YearlyFinancials
            {
                Earnings = earn,
                MetricDict = metricDict,
                Year = e.Year
            });
        }

        /// <summary>
        /// Create ev to ebitda metric and intantiatiate its properties
        /// </summary>
        /// <param name="e"></param>
        /// <param name="result"></param>
        private void CreateEvEbitdaMetric(MetricEventArgs e, double result)
        {
            Earning earn = new Ebitda();
            EnterpriseValue ev = new EnterpriseValue();

            earn.EbitdaValue = e.Ebitda;
            ev.MarketValue = e.MarketValue;
            ev.NetDebt = e.NetDebt;

            Dictionary<MetricType, double> metricDict = new Dictionary<MetricType, double>();
            metricDict[MetricType.EvEbitda] = result;

            e.Stock.Financials.Add(new YearlyFinancials
            {
                Earnings = earn,
                MetricDict = metricDict,
                Year = e.Year,
                EnterpriseVal = ev
            });
        }

        /// <summary>
        /// Create ev to ebit metric and intantiatiate its properties
        /// </summary>
        /// <param name="e"></param>
        /// <param name="result"></param>
        private void CreateEvEbitMetric(MetricEventArgs e, double result)
        {
            Earning earn = new Ebit();
            EnterpriseValue ev = new EnterpriseValue();

            earn.EbitValue = e.Ebit;
            ev.MarketValue = e.MarketValue;
            ev.NetDebt = e.NetDebt;

            Dictionary<MetricType, double> metricDict = new Dictionary<MetricType, double>();
            metricDict[MetricType.EvEbit] = result;

            e.Stock.Financials.Add(new YearlyFinancials
            {
                Earnings = earn,
                MetricDict = metricDict,
                Year = e.Year,
                EnterpriseVal = ev
            });
        }

        /// <summary>
        /// Create net debt to ebitda metric and intantiatiate its properties
        /// </summary>
        /// <param name="e"></param>
        /// <param name="result"></param>
        private void CreateNetDebtToEbitdaMetric(MetricEventArgs e, double result)
        {
            Earning earn = new Ebitda();
            earn.EbitdaValue = e.Ebitda;

            Dictionary<MetricType, double> metricDict = new Dictionary<MetricType, double>();
            metricDict[MetricType.NetDebtToEbitda] = result;

            e.Stock.Financials.Add(new YearlyFinancials
            {
                Earnings = earn,
                MetricDict = metricDict,
                Year = e.Year,
            });
        }

        /// <summary>
        /// Add stock test values
        /// </summary>
        public void AddStockTestValues()
        {
            var stocks = new List<Stock>
            {
                new Stock
                {
                    Name = "Apple",
                    Ticker = "AAPL",
                    Financials = new List<YearlyFinancials>
                    {
                        new YearlyFinancials
                        {
                            Year = 2023,
                            Revenue = 394328, // In millions
                            NmbrOfShares = 16788, // In millions
                            Earnings = new Earning { NetIncomeValue = 94680, EbitValue = 110910, EbitdaValue = 124710 },
                            EnterpriseVal = new EnterpriseValue { MarketValue = 2500000, NetDebt = -57000 }, // Market value in millions
                            MetricDict = new Dictionary<MetricType, double>
                            {
                                { MetricType.PriceToEarnings, 28.94 },
                                { MetricType.EvEbitda, 18.89 }
                            }
                        },
                        new YearlyFinancials
                        {
                            Year = 2022,
                            Revenue = 365817, // In millions
                            NmbrOfShares = 16800, // In millions
                            Earnings = new Earning { NetIncomeValue = 94680, EbitValue = 105000, EbitdaValue = 120000 },
                            EnterpriseVal = new EnterpriseValue { MarketValue = 2200000, NetDebt = -55000 }, // Market value in millions
                            MetricDict = new Dictionary<MetricType, double>
                            {
                                { MetricType.PriceToEarnings, 30.00 },
                                { MetricType.EvEbitda, 19.50 }
                            }
                        }
                    }
                },
                new Stock
                {
                    Name = "Microsoft Corporation",
                    Ticker = "MSFT",
                    Financials = new List<YearlyFinancials>
                    {
                        new YearlyFinancials
                        {
                            Year = 2023,
                            Revenue = 198270, // In millions
                            NmbrOfShares = 7470, // In millions
                            Earnings = new Earning { NetIncomeValue = 61270, EbitValue = 78550, EbitdaValue = 93210 },
                            EnterpriseVal = new EnterpriseValue { MarketValue = 2320000, NetDebt = -55000 }, // Market value in millions
                            MetricDict = new Dictionary<MetricType, double>
                            {
                                { MetricType.PriceToEarnings, 34.12 },
                                { MetricType.EvEbitda, 20.99 }
                            }
                        },
                        new YearlyFinancials
                        {
                            Year = 2022,
                            Revenue = 184900, // In millions
                            NmbrOfShares = 7500, // In millions
                            Earnings = new Earning { NetIncomeValue = 63000, EbitValue = 80000, EbitdaValue = 95000 },
                            EnterpriseVal = new EnterpriseValue { MarketValue = 2100000, NetDebt = -50000 }, // Market value in millions
                            MetricDict = new Dictionary<MetricType, double>
                            {
                                { MetricType.PriceToEarnings, 32.00 },
                                { MetricType.EvEbitda, 19.80 }
                            }
                        }
                    }
                },
                new Stock
                {
                    Name = "Amazon.com",
                    Ticker = "AMZN",
                    Financials = new List<YearlyFinancials>
                    {
                        new YearlyFinancials
                        {
                            Year = 2023,
                            Revenue = 502190, // In millions
                            NmbrOfShares = 10130, // In millions
                            Earnings = new Earning { NetIncomeValue = 33800, EbitValue = 48020, EbitdaValue = 61430 },
                            EnterpriseVal = new EnterpriseValue { MarketValue = 1693000, NetDebt = 2000 }, // Market value in millions
                            MetricDict = new Dictionary<MetricType, double>
                            {
                                { MetricType.PriceToEarnings, 50.14 },
                                { MetricType.EvEbitda, 27.57 }
                            }
                        },
                        new YearlyFinancials
                        {
                            Year = 2022,
                            Revenue = 469820, // In millions
                            NmbrOfShares = 10000, // In millions
                            Earnings = new Earning { NetIncomeValue = 33000, EbitValue = 47000, EbitdaValue = 60000 },
                            EnterpriseVal = new EnterpriseValue { MarketValue = 1500000, NetDebt = 1000 }, // Market value in millions
                            MetricDict = new Dictionary<MetricType, double>
                            {
                                { MetricType.PriceToEarnings, 48.00 },
                                { MetricType.EvEbitda, 26.50 }
                            }
                        }
                    }
                }
            };

            foreach (Stock stock in stocks)
            {
                addItem(stock);
            }
        }
    }
}
