using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockValuationApp.Entities.Stocks;
using StockValuationApp.Main.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Main.Uri
{
    public class UriFinanceManager
    {
        //Given a ticker (string) store jsonobjects (metric data) for a specific period (Annual, Q1, Q2..)
        private Dictionary<string, Dictionary<PeriodTypes, Dictionary<FinanceCategory, List<JObject>>>> metricDict;

        public UriFinanceManager()
        { 
            metricDict = new Dictionary<string, Dictionary <PeriodTypes, Dictionary<FinanceCategory, List<JObject>>>>();
        }

        public async Task<List<JObject>> GetFinanceData(string ticker, FinanceCategory finance, PeriodTypes period)
        {
            List<JObject> jObjs = null;

            if (metricDict.TryGetValue(ticker, out var periodDict) &&
                periodDict.TryGetValue(period, out var financeDict) &&
                financeDict.TryGetValue(finance, out var oldJObj) && oldJObj != null)
            {
                jObjs = oldJObj;
            }
            else
            {
                if (!metricDict.ContainsKey(ticker))
                {
                    metricDict[ticker] = new Dictionary<PeriodTypes, Dictionary<FinanceCategory, List<JObject>>>();
                }

                periodDict = metricDict[ticker];

                if (!periodDict.ContainsKey(period))
                {
                    periodDict[period] = new Dictionary<FinanceCategory, List<JObject>>();
                }

                financeDict = periodDict[period];

                if (!financeDict.ContainsKey(finance))
                {
                    financeDict[finance] = new List<JObject>();
                }
                
                switch (finance)
                {

                    case FinanceCategory.Income:
                        jObjs = await HttpStockMetrics.ImportIncomeMetricData(ticker, period.ToString());
                        break;
                    case FinanceCategory.BalanceSheet:
                        jObjs = await HttpStockMetrics.ImportBalanceSheetMetricData(ticker, period.ToString());
                        break;
                    case FinanceCategory.Cashflow:
                        jObjs = await HttpStockMetrics.ImportCashFlowMetricData(ticker, period.ToString());                     
                        break;
                    case FinanceCategory.StatementAnalysis:
                        jObjs = await HttpStockMetrics.ImportStatementAnalysisData(ticker, period.ToString());
                        break;
                    default:
                        Console.WriteLine("Finance Category is null or does not exist");
                        return default;
                }

                metricDict[ticker][period][finance].AddRange(jObjs);
            }

            return jObjs;
        }
    }
}
