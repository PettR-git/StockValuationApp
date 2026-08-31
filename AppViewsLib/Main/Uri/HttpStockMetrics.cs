using Newtonsoft.Json.Linq;
using StockValuationApp.Main.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StockValuationApp.Main.Uri
{
    /// <summary>
    /// Request a json object for a specific financial category
    /// </summary>
    public static class HttpStockMetrics
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<List<JObject>> ImportMetricData(string ticker, FinanceCategory finance, string period, string apiKey)
        {
            string url = string.Empty;

            switch (finance)
            {
                case FinanceCategory.Income:
                    url = $"https://www.alphavantage.co/query?function=INCOME_STATEMENT&symbol={ticker}&apikey={apiKey}";
                    break;
                case FinanceCategory.BalanceSheet:
                    url = $"https://www.alphavantage.co/query?function=BALANCE_SHEET&symbol={ticker}&apikey={apiKey}";
                    break;
                case FinanceCategory.Cashflow:
                    url = $"https://www.alphavantage.co/query?function=CASH_FLOW&symbol={ticker}&apikey={apiKey}";
                    break;
                case FinanceCategory.StockPrice:

                    //Special case for stock price data, as it requires a different API endpoint and processing
                    return await ImportWeeklyPriceData(ticker, period, apiKey);

                case FinanceCategory.SharesOutstanding:

                    //Special case for shares outstanding data, as it requires a different API endpoint and processing
                    return await ImportSharesOutstandingData(ticker, period, apiKey);

                default:
                    throw new ArgumentException("Invalid finance category");
            }

            //Standard Yearly financial data API call
            string response = await FetchDataFromApiAsync(url);
            JObject jObj = JObject.Parse(response);
            string filterStr = string.Empty;

            if(period == "annual")
            {
                filterStr = "annualReports";
            }
            var jsonObjs = CleanJsonObjects(jObj, period, filterStr);

            return jsonObjs;
        }

        public static async Task<List<JObject>> ImportWeeklyPriceData(string ticker, string period, string apiKey)
        {
            // Close stock price for each year
            string urlPrice = $"https://www.alphavantage.co/query?function=TIME_SERIES_WEEKLY&symbol={ticker}&apikey={apiKey}";

            string priceResponse = await FetchDataFromApiAsync(urlPrice);
            JObject priceObj = JObject.Parse(priceResponse);
            var priceData = CleanPriceObjs(priceObj);

            return priceData;
        }

        public static async Task<List<JObject>> ImportSharesOutstandingData(string ticker, string period, string apiKey)
        {
            // Shares outstanding for each year
            string urlShares = $"https://www.alphavantage.co/query?function=SHARES_OUTSTANDING&symbol={ticker}&apikey={apiKey}";

            string response = await FetchDataFromApiAsync(urlShares);
            JObject shareObj = JObject.Parse(response);
            var shareData = CleanJsonObjects(shareObj, period, "data");

            return shareData;
        }

        private static List<JObject> CleanPriceObjs(JObject jsonObj)
        {
            var result = new List<JObject>();

            // Find the property that contains "Time Series" (covers Daily/Weekly/Monthly variants)
            var seriesProp = jsonObj.Properties()
                .FirstOrDefault(p => p.Name.IndexOf("Time Series", StringComparison.OrdinalIgnoreCase) >= 0);

            if (seriesProp?.Value is not JObject seriesObj)
            {
                return result;
            }

            // Iterate properties (date keys) in descending order
            foreach (var prop in seriesObj.Properties().OrderByDescending(p => p.Name))
            {
                if (!DateTime.TryParse(prop.Name, out var date))
                {
                    continue;
                }

                if (prop.Value is JObject valueObj)
                {
                    // Build a normalized object with desired keys
                    var normalized = new JObject
                    {
                        ["date"] = date.ToString("yyyy-MM-dd")
                    };

                    foreach (var childProp in valueObj.Properties())
                    {
                        var nameLower = childProp.Name.Trim().ToLowerInvariant();

                        if (nameLower.EndsWith("open", StringComparison.OrdinalIgnoreCase))
                        {
                            normalized["openPrice"] = childProp.Value;
                        }
                        else if (nameLower.EndsWith("high", StringComparison.OrdinalIgnoreCase))
                        {
                            normalized["highPrice"] = childProp.Value;
                        }
                        else if (nameLower.EndsWith("low", StringComparison.OrdinalIgnoreCase))
                        {
                            normalized["lowPrice"] = childProp.Value;
                        }
                        else if (nameLower.EndsWith("close", StringComparison.OrdinalIgnoreCase))
                        {
                            normalized["closePrice"] = childProp.Value;
                        }
                        else if (nameLower.EndsWith("volume", StringComparison.OrdinalIgnoreCase))
                        {
                            normalized["volume"] = childProp.Value;
                        }
                        else
                        {
                            // Preserve unexpected keys under their original names
                            if (!normalized.ContainsKey(childProp.Name))
                            {
                                normalized[childProp.Name] = childProp.Value;
                            }
                        }
                    }

                    result.Add(normalized);
                }
            }

            return result;
        }

        private static List<JObject> CleanJsonObjects(JObject jsonObj, string period, string filterStr)
        {
            JArray? jArray = null;
            List<JObject> jObjs = new List<JObject>();

            if (period == "annual")
            {
                jArray = (JArray?)jsonObj[filterStr];

                if (jArray != null)
                {
                    foreach (var item in jArray)
                    {
                        if (item is JObject jObject)
                        {
                            if (jObject.TryGetValue("fiscalDateEnding", out JToken? dateValue))
                            {
                                jObject["date"] = dateValue;
                                jObject.Remove("fiscalDateEnding");
                            }
                            jObjs.Add(jObject);
                        }
                    }
                }
            }

            return jObjs;
        }

        private static async Task<string> FetchDataFromApiAsync(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
