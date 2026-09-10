using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using OpenTK.Audio.OpenAL;
using StockLib.Main.Calculations;
using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockLib.Main.Agents.Analyst.Technical
{
    public class TechnicalAnalysisAgent : StockAgentBase
    {
        public override string AgentName => "Technical Analysis Agent";

        public TechnicalAnalysisAgent(Kernel? kernel = null) : base(kernel) { }

        protected override string SystemPrompt =>
            "You are a quantitative technical analysis researcher. " +
            "Evaluate trend alignment, support/resistance, and momentum metrics. " +
            "Return your response ONLY as a valid JSON object.\n\n" +
            "Exact JSON structure:\n" +
            "{\n" +
            "  \"trendOutlook\": \"<Trend breakdown>\",\n" +
            "  \"keySupportResistance\": \"<Levels evaluation>\",\n" +
            "  \"mediumTermVerdict\": \"<Bullish/Bearish/Neutral>\"\n" +
            "  \"longTermVerdict\": \"<Bullish/Bearish/Neutral>\"\n" +
            "}";

        protected override string BuildUserPrompt(Stock stock)
        {
            var prices = stock?.WeeklyPrices;

            if (prices == null || !prices.Any())
            {
                return "No weekly prices available for analysis.";
            }

            TechnicalIndicatorCalculator calc = new TechnicalIndicatorCalculator(prices);

            decimal currentPrice = Convert.ToDecimal(prices.OrderBy(p => p.Timestamp).Last().Close);
            double? ema20 = calc.GetEma(20);
            double? ema50 = calc.GetEma(50);
            double? sma200 = calc.GetSma(200);

            double? rsi14 = calc.GetRsi(14);
            var (macdLine, signalLine, histogram) = calc.GetMacd(12, 26, 9);
            double? atr14 = calc.GetAtr(14);
            double? volumeRatio = calc.GetVolume20Ratio();

            List<decimal> resistances = calc.GetKeyResistances(window: 3, take: 3);
            List<decimal> supports = calc.GetKeySupports(window: 3, take: 3);

            // Evaluate moving average alignment
            string alignment = EvaluateAlignment(currentPrice, ema20, ema50, sma200);

            // Build structured payload
            var payload = new
            {
                ticker = stock?.Ticker,
                currentPrice = Math.Round(currentPrice, 2),
                trend = new
                {
                    ema20 = ema20.HasValue ? Math.Round(ema20.Value, 2) : (double?)null,
                    ema50 = ema50.HasValue ? Math.Round(ema50.Value, 2) : (double?)null,
                    sma200 = sma200.HasValue ? Math.Round(sma200.Value, 2) : (double?)null,
                    alignment = alignment
                },
                momentumAndVolatility = new
                {
                    rsi14 = rsi14.HasValue ? Math.Round(rsi14.Value, 2) : (double?)null,
                    macd = new
                    {
                        line = macdLine.HasValue ? Math.Round(macdLine.Value, 2) : (double?)null,
                        signal = signalLine.HasValue ? Math.Round(signalLine.Value, 2) : (double?)null,
                        histogram = histogram.HasValue ? Math.Round(histogram.Value, 2) : (double?)null
                    },
                    atr14 = atr14.HasValue ? Math.Round(atr14.Value, 2) : (double?)null,
                    volumeVs20WeekAvgRatio = volumeRatio
                },
                keyLevels = new
                {
                    support = supports.Select(s => Math.Round(s, 2)).ToList(),
                    resistance = resistances.Select(r => Math.Round(r, 2)).ToList()
                }
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            return $"Analyze the following pre-computed technical indicators for {stock?.Ticker}:\n\n" +
                   JsonSerializer.Serialize(payload, options);
        }

        private string EvaluateAlignment(decimal price, double? ema20, double? ema50, double? sma200)
        {
            if (!ema20.HasValue || !ema50.HasValue || !sma200.HasValue)
                return "Insufficient Data";

            double p = (double)price;

            if (p > ema20 && ema20 > ema50 && ema50 > sma200)
                return "Bullish Stack (Price > EMA20 > EMA50 > SMA200)";

            if (p < ema20 && ema20 < ema50 && ema50 < sma200)
                return "Bearish Stack (Price < EMA20 < EMA50 < SMA200)";

            if (p > sma200)
                return "Long-Term Bullish / Short-Term Consolidating";

            return "Mixed / Neutral";
        }
    }
}
