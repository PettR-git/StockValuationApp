using Microsoft.SemanticKernel;
using Newtonsoft.Json.Linq;
using StockLib.Abstraction;
using StockLib.Main.Agents;
using StockLib.Main.Agents.Analyst;
using StockLib.Main.Agents.Analyst.Technical;
using StockLib.Main.Entities.Stocks;
using StockLib.Main.Entities.Stocks.Metrics;
using StockValuationApp.Entities.Calculations;
using StockValuationApp.Entities.Enums;
using StockValuationApp.Entities.Stocks.Metrics;
using StockValuationApp.Entities.Stocks.Metrics.Earnings;
using StockValuationApp.Main.Enums;
using StockValuationApp.Main.Uri;
using WTS.Entities.Main;

namespace StockValuationApp.Entities.Stocks
{
    public class StockManager : ListManager<Stock>
    {
        private UriFinanceManager uriManager;
        private Dictionary<FinanceCategory, List<MetricDataTypes>> metricTemplate;

        private readonly IStockRepository _repo;
        private bool _isAgentAnalyzing;
        private const int maxYearIndex = 10; // Number of years to fetch metrics for
        private const int maxNoRes = 5; // Maximum number of categories that can return no results before skipping the year

        public StockManager(IStockRepository repo)
        {
            _repo = repo;
            uriManager = new UriFinanceManager();
            metricTemplate = BuildMetricTemplate();
        }

        // load/save using _repo
        public async Task LoadAllAsync()
        {
            var list = await _repo.LoadAllAsync();
            Clear();
            AddRange(list);
        }

        public async Task SaveAllAsync()
        {
            await _repo.SaveAllAsync(this.ToList());
        }

        /// <summary>
        /// Create stock object and initialize name and ticker
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ticker"></param>
        /// <returns>stock object</returns>
        public Stock? CreateStock(string name, string ticker)
        {
            if (string.IsNullOrEmpty(ticker) || string.IsNullOrEmpty(name))
                return null;

            Stock stock = new Stock();
            stock.Name = name;
            stock.Ticker = ticker;
            stock.StockScore = new StockScore();

            return stock;
        }

        /// <summary>
        /// If financials for a specific year exist return true
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="year"></param>
        /// <param name="metricType"></param>
        /// <returns>boolean</returns>
        private (bool, YearlyFinancials?) DoesFinanceObjExistFrYear(Stock stock, int year)
        {
            bool yearExist = false;
            YearlyFinancials? yearlyFinancials = null;

            foreach (var yf in stock.YearlyFinancials)
            {
                if (yf != null && yf.Year == year)
                {
                    yearExist = true;
                    yearlyFinancials = yf;
                    break;
                }
            }

            return(yearExist, yearlyFinancials);
        }

        public bool AddWeeklyPriceDataFrStock(WeeklyStockPricesEventArgs e)
        {
            Stock? stock = this.FirstOrDefault(s => s.Ticker == e.Symbol);

            if (stock == null)
            {
                Console.WriteLine($"Stock with ticker {e.Symbol} not found.");
                return false;
            }

            stock.WeeklyPrices = e.Bars.ToList();
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
            Stock? stock = e.Stock;

            if (stock == null)
            {
                Console.WriteLine($"Stock with ticker {e.Stock?.Ticker} not found.");
                return false;
            }

            int year = e.Year;
            decimal keyFigureVal = default;

            List<KeyFigureTypes> keyFigureTypes = Enum.GetValues(typeof(KeyFigureTypes)).Cast<KeyFigureTypes>().ToList();
            var evTuple = (e.MarketValue, e.ShortTermDebt, e.LongTermDebt, e.CashAndEquivalents);
            var netDebtTuple = (e.ShortTermDebt, e.LongTermDebt, e.CashAndEquivalents);

            //Calculate specific metric and determine if object creation or update is needed
            (bool yfExist, YearlyFinancials? yf) = DoesFinanceObjExistFrYear(stock, year);

            if (yf == null)
            {
                yf = new YearlyFinancials();
            }

            yf.Year = year;
            if (year > DateTime.Now.Year)
            {
                yf.IsEstimate = true;
            }
            //Earnings
            yf.Earnings = new Earning();
            yf.Earnings.EbitValue = e.Ebit;
            yf.Earnings.EbitdaValue = e.Ebitda;
            yf.Earnings.NetIncomeValue = e.NetIncome;
            yf.Revenue = e.Revenue;
            yf.Earnings.EbitdaMargin = (decimal)Math.Round(100 * (e.Ebitda/e.Revenue), 1);
            yf.Earnings.EbitMargin = (decimal)Math.Round(100 * (e.Ebit/e.Revenue), 1);
            yf.Earnings.NetIncomeMargin = (decimal)Math.Round(100 * (e.NetIncome/e.Revenue), 1);

            //Enterprise value
            yf.EnterpriseVal = new EnterpriseValue();
            yf.EnterpriseVal.MarketValue = e.MarketValue;
            yf.EnterpriseVal.LongTermDebt = e.LongTermDebt;
            yf.EnterpriseVal.ShortTermDebt = e.ShortTermDebt;
            yf.EnterpriseVal.CashAndEquivalents = e.CashAndEquivalents;

            //Other financials
            yf.NmbrOfShares = e.SharesOutstanding;
            yf.StockPrice = e.YearEndClosePrice;
            yf.Dividends = e.Dividends;
            yf.OperatingCashFlow = e.OperationalCashflow;
            yf.CapitalExpenditures = e.CapitalExpenditures;

            //@TODO Add more stock financials

            if (yfExist == false)
            {
                stock.YearlyFinancials.Add(yf);
            }

            SortKeyFinancialsByYear(stock);

            foreach (var keyFigureType in keyFigureTypes)
            {
                switch (keyFigureType)
                {
                    case KeyFigureTypes.ReturnOnInvCap:

                        keyFigureVal = CalculateKeyFigure.CalcRoic(e.NetIncome, e.Dividends, e.LongTermDebt, e.ShortTermDebt, e.TotalAssets, e.TotalLiabilities);
                        break;

                    case KeyFigureTypes.EvFreecashflow:

                        keyFigureVal = CalculateKeyFigure.CalcEvFreeCashflow(evTuple, e.OperationalCashflow, e.CapitalExpenditures);
                        break;

                    case KeyFigureTypes.FreeCashflow:

                        keyFigureVal = (decimal)(e.OperationalCashflow - e.CapitalExpenditures);
                        break;

                    case KeyFigureTypes.ReturnOnEquity:

                        keyFigureVal = CalculateKeyFigure.CalcRoe(e.NetIncome, e.TotalAssets, e.TotalLiabilities);
                        break;

                    case KeyFigureTypes.EvEbitda:

                        keyFigureVal = CalculateKeyFigure.CalcEvEarnings(evTuple, e.Ebitda);
                        break;

                    case KeyFigureTypes.EvEbit:

                        keyFigureVal = CalculateKeyFigure.CalcEvEarnings(evTuple, e.Ebit);
                        break;

                    case KeyFigureTypes.PriceToEarnings:

                        keyFigureVal = CalculateKeyFigure.CalcPriceToEarnings((e.NetIncome, e.SharesOutstanding), e.YearEndClosePrice);
                        break;

                    case KeyFigureTypes.NetDebtToEbitda:

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
        private void UpdateOrCreateKeyFigure(Stock stock, YearlyFinancials yf, KeyFigureTypes keyFigure, decimal result)
        {
            int index = stock.YearlyFinancials.IndexOf(yf);

            //if keyfigure dictionary doesnt exist, create dictionary and keyfigure
            if(stock.YearlyFinancials[index].KeyFiguresDict == null)
            {
                Dictionary<KeyFigureTypes, decimal> metricDict = new Dictionary<KeyFigureTypes, decimal>();
                metricDict[keyFigure] = result;

                stock.YearlyFinancials[index].KeyFiguresDict = metricDict;
            }
            //if keyfigure in keyfigure dictionary doesnt exist, create keyfigure
            else if (!stock.YearlyFinancials[index].KeyFiguresDict.ContainsKey(keyFigure))
            {
                stock.YearlyFinancials[index].KeyFiguresDict.Add(keyFigure, result);
            }
            //keyfigure in keyfigure dictionary exist, update it
            else
            {
                stock.YearlyFinancials[index].KeyFiguresDict[keyFigure] = result;
            }
        }

        public void SortKeyFinancialsByYear(Stock stock)
        {
            stock.YearlyFinancials = stock.YearlyFinancials.OrderBy(x => x.Year).ToList();
        }

        /// <summary>
        /// Categorize metrics depending on category that the Api is using
        /// </summary>
        /// <summary>
        /// Build a single, reusable metric template mapping finance category -> metrics.
        /// </summary>
        private static Dictionary<FinanceCategory, List<MetricDataTypes>> BuildMetricTemplate()
        {
            return new Dictionary<FinanceCategory, List<MetricDataTypes>>
            {
                [FinanceCategory.Income] = new List<MetricDataTypes>
                {
                    MetricDataTypes.netIncome,
                    MetricDataTypes.ebitda,
                    MetricDataTypes.totalRevenue,
                    MetricDataTypes.ebit,                  
                },
                [FinanceCategory.BalanceSheet] = new List<MetricDataTypes>
                {
                    MetricDataTypes.cashAndCashEquivalentsAtCarryingValue,
                    MetricDataTypes.totalAssets,
                    MetricDataTypes.totalLiabilities,
                    MetricDataTypes.longTermDebt,
                    MetricDataTypes.shortTermDebt,
                },
                [FinanceCategory.Cashflow] = new List<MetricDataTypes>
                {
                    MetricDataTypes.operatingCashFlow,
                    MetricDataTypes.capitalExpenditures,
                    MetricDataTypes.dividendPayout,
                },
                [FinanceCategory.StockPrice] = new List<MetricDataTypes>
                {
                    MetricDataTypes.closePrice,
                    MetricDataTypes.openPrice,
                    MetricDataTypes.highPrice,
                    MetricDataTypes.lowPrice,
                    MetricDataTypes.volume,
                    MetricDataTypes.date
                },
                [FinanceCategory.SharesOutstanding] = new List<MetricDataTypes>
                {
                    MetricDataTypes.shares_outstanding_basic
                }
            };
        }

        /// <summary>
        /// Retrieve metrics for a stock. Fetches all finance categories, 
        /// then assembles yearly MetricEventArgs.
        /// </summary>
        public async Task GetMetricVals(Stock stock)
        {
            if (stock == null) throw new ArgumentNullException(nameof(stock));

            // Cache the current year 
            int currentYear = DateTime.Now.Year;

            for (int i = 0; i < maxYearIndex; i++)
            {
                var args = new MetricEventArgs { Stock = stock };
                int noResCounter = 0;

                int targetYear = currentYear - i - 1;
                string targetYearStr = targetYear.ToString();

                foreach (var kv in metricTemplate)
                {
                    var category = kv.Key;
                    var metrics = kv.Value;
                    List<JObject>? jObjs = null;

                    try
                    {
                        var data = await uriManager.GetFinanceData(stock.Ticker, category, PeriodTypes.annual) ?? new List<JObject>();

                        jObjs = data;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching data for category {category}: {ex.Message}");
                    }

                    // Match by year 
                    var jObjYearly = jObjs?.FirstOrDefault(x => x["date"]?.ToString().StartsWith(targetYearStr) == true);

                    if (jObjYearly == null)
                    {
                        noResCounter++;
                        continue;
                    }

                    //Assigning yearly metrics 
                    foreach (var met in metrics)
                    {
                        try
                        {   
                            var tokenYearly = jObjYearly[met.ToString()];
                            if (tokenYearly == null || !double.TryParse(tokenYearly.ToString(), out double metricVal))
                            {
                                Console.WriteLine($"Metric none or invalid: {met} at year {targetYear}");
                                metricVal = 0;
                            }

                            AssignYearlyMetric(args, met, metricVal);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error parsing metric {met} for category {category} at year {targetYear}: {ex.Message}");
                        }
                    }

                    //Assign Daily stock prices
                    if (category == FinanceCategory.StockPrice)
                    {
                        AssignWeeklyPrices(jObjs, stock);
                    }
                }

                args.Year = targetYear;

                if (noResCounter < maxNoRes)
                {
                    stock.MetricsGiven?.Invoke(this, args);
                }
            }
        }

        private void AssignWeeklyPrices(List<JObject>? jObjs, Stock stock)
        {
            if(jObjs == null || jObjs.Count == 0)
            {
                Console.WriteLine($"No daily price data available for stock {stock.Ticker}");
                return;
            }   

            // Populate the StockBar collection with Jobjs
            List<StockBar> bars = jObjs.Select(jObj => new StockBar(
                Symbol: stock.Ticker,
                Timestamp: (DateTimeOffset)jObj[MetricDataTypes.date.ToString()]!, 
                Open: (decimal)jObj[MetricDataTypes.openPrice.ToString()]!,
                High: (decimal)jObj[MetricDataTypes.highPrice.ToString()]!,
                Low: (decimal)jObj[MetricDataTypes.lowPrice.ToString()]!,
                Close: (decimal)jObj[MetricDataTypes.closePrice.ToString()]!,
                Volume: (long)jObj[MetricDataTypes.volume.ToString()]!
            )).ToList();

            // Ensure chronological ordering for chart rendering (oldest -> newest)
            IReadOnlyList<StockBar> sortedBars = bars.OrderBy(b => b.Timestamp).ToList();

            // Calculate date boundaries automatically from the populated list
            DateTimeOffset startDate = sortedBars.Count > 0 ? sortedBars[0].Timestamp : DateTimeOffset.MinValue;
            DateTimeOffset endDate = sortedBars.Count > 0 ? sortedBars[sortedBars.Count - 1].Timestamp : DateTimeOffset.MaxValue;

            var args = new WeeklyStockPricesEventArgs(stock.Ticker, startDate, endDate, sortedBars);

            OnWeeklyPricesLoaded(args, stock);     
        }

        //Thread-safe virtual OnEvent method 
        protected virtual void OnWeeklyPricesLoaded(WeeklyStockPricesEventArgs e, Stock stock)
        {          
            stock.WeeklyStockPricesGiven?.Invoke(this, e);
        }

        /// <summary>
        /// Assign parsed metric value to the appropriate MetricEventArgs property.
        /// Keep assignments centralized to avoid duplication.
        /// </summary>
        private static void AssignYearlyMetric(MetricEventArgs args, MetricDataTypes met, double metricVal)
        {
            switch (met)
            {
                case MetricDataTypes.operatingCashFlow:
                    args.OperationalCashflow = metricVal;
                    break;
                case MetricDataTypes.dividendPayout:
                    args.Dividends = -metricVal;
                    break;
                case MetricDataTypes.capitalExpenditures:
                    args.CapitalExpenditures = -metricVal;
                    break;

                case MetricDataTypes.shares_outstanding_basic:
                    args.SharesOutstanding = metricVal;
                    break;
                case MetricDataTypes.closePrice:
                    args.YearEndClosePrice = metricVal;
                    break;

                case MetricDataTypes.totalRevenue:
                    args.Revenue = metricVal;
                    break;
                case MetricDataTypes.ebitda:
                    args.Ebitda = metricVal;
                    break;
                case MetricDataTypes.ebit:
                    args.Ebit = metricVal;
                    break;
                case MetricDataTypes.netIncome:
                    args.NetIncome = metricVal;
                    break;

                case MetricDataTypes.cashAndCashEquivalentsAtCarryingValue:
                    args.CashAndEquivalents = metricVal;
                    break;
                case MetricDataTypes.totalAssets:
                    args.TotalAssets = metricVal;
                    break;
                case MetricDataTypes.totalLiabilities:
                    args.TotalLiabilities = metricVal;
                    break;
                case MetricDataTypes.longTermDebt:
                    args.LongTermDebt = metricVal;
                    break;
                case MetricDataTypes.shortTermDebt:
                    args.ShortTermDebt = metricVal;
                    break;

                default:
                    break;
            }
        }

        public async Task RunAgentAnalysisAsync(Stock stock, StockAgentBase agent)
        {
            IsAgentAnalyzing = true;

            try
            {
                string analysisResult = await Task.Run(async () =>
                    await agent.AnalyzeAsync(stock)
                );

                SetJsonOutput(stock, agent.AgentName, analysisResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                IsAgentAnalyzing = false;
            }
        }

        public async Task RunAllAgentsSequentiallyAsync(Stock stock)
        {
            if (stock == null)
            {
                return;
            }

            IsAgentAnalyzing = true;

            var builder = Kernel.CreateBuilder();
            builder.AddOllamaChatCompletion("llama3.1:8b", new Uri("http://127.0.0.1:11434"));
            var sharedKernel = builder.Build();

            var agents = new List<StockAgentBase>
            {
                new StockAnalysisOverviewAgent(sharedKernel),
                new TechnicalAnalysisAgent(sharedKernel)
            };

            var queueRunner = new AgentQueueRunner(agents);

            // Process one by one in sequence
            await queueRunner.RunQueueAsync(stock, (agentName, jsonOutput) =>
            {
                // Fires as each agent finishes
                Console.WriteLine($"[Completed {agentName}]: {jsonOutput}");

                SetJsonOutput(stock, agentName, jsonOutput);
            });

            IsAgentAnalyzing = false;
        }

        private void SetJsonOutput(Stock stock, string agentName, string jsonOutput)
        {
            switch(agentName)
            {
                case "Stock Overview Agent":
                    stock.OverviewAgentJson = jsonOutput;
                    break;
                case "Technical Analysis Agent":
                    stock.TechnicalAnalysisAgentJson = jsonOutput;
                    break;
                default:
                    Console.WriteLine($"Unknown agent: {agentName}");
                    break;
            }
        }

        public bool IsAgentAnalyzing
        {
            get => _isAgentAnalyzing;
            set { _isAgentAnalyzing = value; }
        }
    }
}
