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
        public Stock CreateStock(string name, string ticker)
        {
            if (string.IsNullOrEmpty(ticker) || string.IsNullOrEmpty(name))
                return null;

            Stock stock = new Stock();
            stock.Name = name;
            stock.Ticker = ticker;

            return stock;
        }


        private bool DoesFinanceObjExistFrYear(Stock stock, string year, MetricType metricType)
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

        public bool AddMetricDataFrStock(MetricEventArgs e)
        {
            MetricType metricType = e.MetricType;
            Stock stock = e.Stock;
            String year = e.Year;
            double metricVal = 0.0;

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

        public void AddStockTestValues()
        {
            List<Stock> stocks = new List<Stock> { 
                new Stock
                {
                    Name = "Apple",
                    Ticker = "APPL",
                },
                new Stock
                {
                    Name = "Atlas Copco",
                    Ticker = "ATCO"
                }
            };

            foreach(Stock stock in stocks)
            {
                addItem(stock);
            }
        }

        /*private void AddFinancialTestValues()
        {
            Stock stock = null;
            for (int i = 0; i<Count(); i++)
            {
                stock = getListItemAt(i);
                stock.Financials.Add(new YearlyFinancials
                {
                    Year = "2022",
                    Earnings = new NetIncome { NetIncomeValue = 99},
                    EnterpriseVal = new EnterpriseValue { 
                        MarketValue = 2930000,
                        NetDebt = -36000,
                    },
                    MetricDict = new Dictionary<MetricType, double>().Add(MetricType.EvEbitda, CalculateValuationMetric.CalcEvEarnings((Ear)))
                    
                });
            }
        }*/

    }
}
