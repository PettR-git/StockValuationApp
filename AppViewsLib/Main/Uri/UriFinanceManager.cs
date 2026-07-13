using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockValuationApp.Entities.Stocks;
using StockValuationApp.Main.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Main.Uri
{
    public class UriFinanceManager
    {
        //Given a ticker (string) store jsonobjects (metric data) for a specific period (Annual, Q1, Q2..)
        private Dictionary<string, Dictionary<PeriodTypes, Dictionary<FinanceCategory, List<JObject>>>> metricDict;
        private List<string> apiKeys;
        private int keyNmbr;
        private int apiCallTries;

        public UriFinanceManager()
        {
            apiKeys = ["MHDCCRC0NHNSOO5C","DFD8Y3IB61SDY637","BEM6HGW60QG982T7"];
            metricDict = new Dictionary<string, Dictionary <PeriodTypes, Dictionary<FinanceCategory, List<JObject>>>>();
            keyNmbr = 0;
            apiCallTries = 0;
        }

        public async Task<List<JObject>?> GetFinanceData(string ticker, FinanceCategory category, PeriodTypes period)
        {
            List<JObject> jObjs = null;

            if (metricDict.TryGetValue(ticker, out var periodDict) &&
                periodDict.TryGetValue(period, out var financeDict) &&
                financeDict.TryGetValue(category, out var oldJObj) && oldJObj != null)
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

                if (!financeDict.ContainsKey(category))
                {
                    financeDict[category] = new List<JObject>();
                }

                int faultyTries = 0, maxTries = 1;
                bool success = true;

                do
                {
                    try
                    {
                        jObjs = await HttpStockMetrics.ImportMetricData(ticker, category, period.ToString().ToLower(), apiKeys[keyNmbr]);
                        Debug.WriteLine("Number of total tried API-calls: " + ++apiCallTries);
                        
                        metricDict[ticker][period][category].AddRange(jObjs);
                        // Rate limit: wait 1 second between requests
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    catch (HttpRequestException hrex)
                    {
                        Console.WriteLine("Faulty HTTP request" + hrex.Message);
                        
                        if(keyNmbr < apiKeys.Count - 1)
                        {
                            keyNmbr++;
                            Console.WriteLine("Switching to next API key: " + apiKeys[keyNmbr]);
                        }
                        else
                        {
                            Console.WriteLine("All API keys exhausted. Please try again later.");
                            return default;
                        }
                        faultyTries++;
                        success = false;
                    }
                    catch(ArgumentNullException anex)
                    {
                        Console.WriteLine("Argument is null, likely JObjs" + anex.Message);
                        faultyTries++;
                        success = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        faultyTries++;
                        success = false;
                    }

                } while (faultyTries <= maxTries && !success);
            }

            if (jObjs == null)
                return new List<JObject>();

            return jObjs;
        }
    }
}
