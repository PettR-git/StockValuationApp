using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using StockValuationApp.Entities.Enums;
using StockValuationApp.Entities.Stocks.Metrics.Earnings;

namespace StockValuationApp.Entities.Stocks.Metrics
{
    /// <summary>
    /// Holds all data for metrics in a specific year.
    /// </summary>
    [Serializable]
    public class YearlyFinancials
    {
        private Dictionary<KeyFigureTypes, decimal> _keyFiguresDict = new();
        private string? _keyFiguresJson;

        public YearlyFinancials()
        {
            Earnings = new Earning();
            EnterpriseVal = new EnterpriseValue();
        }

        public Dictionary<KeyFigureTypes, decimal> KeyFiguresDict { get; set; } = new();

        public string? KeyFiguresJson { get; set; }

        public int Id { get; set; } //Primary key
        public int StockId { get; set; } //Foreign key to the Stock entity

        public int Year { get; set; }
        public bool IsEstimate { get; set; } = false;
        public double Revenue { get; set; } = 0;
        public double NmbrOfShares { get; set; } = 0;
        public double StockPrice { get; set; } = 0;
        public double OperatingCashFlow { get; set; } = 0;
        public double Dividends { get; set; } = 0;
        public double CapitalExpenditures { get; set; } = 0;
        public Earning? Earnings { get; set; }
        public EnterpriseValue? EnterpriseVal { get; set; }

        public override string ToString()
        {
            string outStr = $"Year {Year}\n";
            string metricStr = string.Empty;

            if (KeyFiguresDict.Count > 0)
            {
                foreach (var kvp in KeyFiguresDict)
                {
                    switch (kvp.Key)
                    {
                        case KeyFigureTypes.EvEbitda:
                            metricStr = "EV/EBITDA";
                            break;
                        case KeyFigureTypes.EvEbit:
                            metricStr = "EV/EBIT";
                            break;
                        case KeyFigureTypes.PriceToEarnings:
                            metricStr = "P/E";
                            break;
                        case KeyFigureTypes.NetDebtToEbitda:
                            metricStr = "Net Debt/EBITDA";
                            break;
                        case KeyFigureTypes.EvFreecashflow:
                            metricStr = "EV/FCF";
                            break;
                        case KeyFigureTypes.ReturnOnInvCap:
                            metricStr = "ROIC";
                            break;
                        case KeyFigureTypes.ReturnOnEquity:
                            metricStr = "ROE";
                            break;
                        default:
                            continue;
                    }

                    if (Year > DateTime.Now.Year)
                        outStr += "Estimation | ";

                    outStr += $"{metricStr}: {kvp.Value:F2} \n";
                }
            }
            else
            {
                outStr = $"Insufficient data for key figures | Year {Year}";
            }

            return outStr.TrimEnd();
        }

        private static string? SerializeKeyFigures(Dictionary<KeyFigureTypes, decimal>? figures)
        {
            if (figures == null || figures.Count == 0)
            {
                return null;
            }

            var payload = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (var kvp in figures)
            {
                payload[kvp.Key.ToString()] = kvp.Value;
            }

            return JsonSerializer.Serialize(payload);
        }

        private static Dictionary<KeyFigureTypes, decimal> DeserializeKeyFigures(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new Dictionary<KeyFigureTypes, decimal>();
            }

            try
            {
                var payload = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json);
                if (payload == null)
                {
                    return new Dictionary<KeyFigureTypes, decimal>();
                }

                var result = new Dictionary<KeyFigureTypes, decimal>();
                foreach (var kvp in payload)
                {
                    if (Enum.TryParse<KeyFigureTypes>(kvp.Key, true, out var keyFigureType))
                    {
                        result[keyFigureType] = kvp.Value;
                    }
                }

                return result;
            }
            catch
            {
                return new Dictionary<KeyFigureTypes, decimal>();
            }
        }
    }
}
