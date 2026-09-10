using Skender.Stock.Indicators;
using StockLib.Main.Entities.Stocks.Metrics;

namespace StockLib.Main.Calculations
{
    public class TechnicalIndicatorCalculator
    {
        private readonly List<Quote> _quotes;

        public TechnicalIndicatorCalculator(IEnumerable<StockBar> stockBars)
        {
            // Convert and sort once upon instantiation
            _quotes = stockBars
                .Select(o => new Quote
                {
                    Date = o.Timestamp.DateTime, // Use o.Timestamp directly if already DateTime/DateTimeOffset
                    Open = Convert.ToDecimal(o.Open),
                    High = Convert.ToDecimal(o.High),
                    Low = Convert.ToDecimal(o.Low),
                    Close = Convert.ToDecimal(o.Close),
                    Volume = Convert.ToDecimal(o.Volume)
                })
                .OrderBy(q => q.Date)
                .ToList();
        }

        public double? GetSma(int lookbackPeriods = 200)
        {
            return _quotes.GetSma(lookbackPeriods).LastOrDefault()?.Sma;
        }

        public double? GetEma(int lookbackPeriods = 50)
        {
            return _quotes.GetEma(lookbackPeriods).LastOrDefault()?.Ema;
        }

        public double? GetRsi(int lookbackPeriods = 14)
        {
            return _quotes.GetRsi(lookbackPeriods).LastOrDefault()?.Rsi;
        }

        public double? GetAtr(int lookbackPeriods = 14) =>
            _quotes.GetAtr(lookbackPeriods).LastOrDefault()?.Atr;

        public double? GetVolume20Ratio()
        {
            if (_quotes.Count < 20) return null;

            decimal lastVolume = _quotes.Last().Volume;
            decimal avg20Volume = _quotes.TakeLast(20).Average(q => q.Volume);

            return avg20Volume > 0 ? (double)Math.Round(lastVolume / avg20Volume, 2) : null;
        }

        public (double? Line, double? Signal, double? Histogram) GetMacd(int fastPeriods = 12, int slowPeriods = 26, int signalPeriods = 9)
        {
            var latest = _quotes.GetMacd(fastPeriods, slowPeriods, signalPeriods).LastOrDefault();
            return (latest?.Macd, latest?.Signal, latest?.Histogram);
        }

        /// <summary>
        /// Identifies major swing highs acting as resistance levels near or above current price.
        /// </summary>
        public List<decimal> GetKeyResistances(int window = 3, int take = 3)
        {
            if (_quotes.Count < window * 2 + 1) return new List<decimal>();

            var pivots = new List<decimal>();

            for (int i = window; i < _quotes.Count - window; i++)
            {
                decimal currentHigh = _quotes[i].High;
                bool isPivot = true;

                for (int offset = -window; offset <= window; offset++)
                {
                    if (offset == 0) continue;
                    if (_quotes[i + offset].High >= currentHigh)
                    {
                        isPivot = false;
                        break;
                    }
                }

                if (isPivot) pivots.Add(currentHigh);
            }

            decimal currentPrice = _quotes.Last().Close;

            return pivots
                .Where(p => p >= currentPrice * 0.95m)
                .OrderByDescending(p => p)
                .Distinct()
                .Take(take)
                .ToList();
        }

        /// <summary>
        /// Identifies major swing lows acting as support levels near or below current price.
        /// </summary>
        public List<decimal> GetKeySupports(int window = 3, int take = 3)
        {
            if (_quotes.Count < window * 2 + 1) return new List<decimal>();

            var pivots = new List<decimal>();

            for (int i = window; i < _quotes.Count - window; i++)
            {
                decimal currentLow = _quotes[i].Low;
                bool isPivot = true;

                for (int offset = -window; offset <= window; offset++)
                {
                    if (offset == 0) continue;
                    if (_quotes[i + offset].Low <= currentLow)
                    {
                        isPivot = false;
                        break;
                    }
                }

                if (isPivot) pivots.Add(currentLow);
            }

            decimal currentPrice = _quotes.Last().Close;

            return pivots
                .Where(p => p <= currentPrice * 1.05m)
                .OrderBy(p => p)
                .Distinct()
                .Take(take)
                .ToList();
        }
    }
}
