using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StockValuationApp.Main.Uri
{
    /// <summary>
    /// Request a json object for a specific financial category
    /// </summary>
    public static class HttpStockMetrics
    {
        private static readonly HttpClient client = new HttpClient();
        private const string apiKey = "wYAeK4VcdGgjyULYbg6kXxwlRQwqJlOM";
        public static async Task<List<JObject>> ImportIncomeMetricData(string ticker, string period)
        {
            string url = $"https://financialmodelingprep.com/api/v3/income-statement/{ticker}?period={period}&apikey={apiKey}";
            List<JObject> jObjs = new List<JObject>();

            try
            {
                string response = await FetchDataFromApiAsync(url);
                JArray jArray = JArray.Parse(response); 

                foreach(var item in jArray)
                {
                    if(item is JObject jObject)
                    {
                        jObjs.Add(jObject);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return jObjs;
        }
        

        public static async Task<List<JObject>> ImportBalanceSheetMetricData(string ticker, string period)
        {
            string url = $"https://financialmodelingprep.com/api/v3/balance-sheet-statement/{ticker}?period={period}&apikey={apiKey}";
            List<JObject> jObjs = new List<JObject>();

            try
            {
                string response = await FetchDataFromApiAsync(url);
                JArray jArray = JArray.Parse(response);

                foreach (var item in jArray)
                {
                    if (item is JObject jObject)
                    {
                        jObjs.Add(jObject);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return jObjs;
        }

        public static async Task<List<JObject>> ImportCashFlowMetricData(string ticker, string period)
        {
            string url = $"https://financialmodelingprep.com/api/v3/income-statement/{ticker}?period={period}&apikey={apiKey}";
            List<JObject> jObjs = new List<JObject>();

            try
            {
                string response = await FetchDataFromApiAsync(url);
                JArray jArray = JArray.Parse(response);

                foreach (var item in jArray)
                {
                    if (item is JObject jObject)
                    {
                        jObjs.Add(jObject);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return jObjs;
        }

        public static async Task<List<JObject>> ImportStatementAnalysisData(string ticker, string period)
        {
            string url = $"https://financialmodelingprep.com/api/v3/enterprise-values/{ticker}?period={period}&apikey={apiKey}";
            List<JObject> jObjs = new List<JObject>();

            try
            {
                string response = await FetchDataFromApiAsync(url);
                JArray jArray = JArray.Parse(response);

                foreach (var item in jArray)
                {
                    if (item is JObject jObject)
                    {
                        jObjs.Add(jObject);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
