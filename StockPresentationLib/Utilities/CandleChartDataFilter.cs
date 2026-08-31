using StockLib.Main.Entities.Stocks.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockPresentationLib.Utilities
{
    public enum TimeInterval
    {
        OneMonth,
        ThreeMonths,
        SixMonths,
        OneYear,
        TwoYears,
        FiveYears,
        TenYears,
        All
    }

    public class CandleChartDataFilter
    {
        /// <summary>
        /// Filters StockBar data based on the selected time interval
        /// </summary>
        public static IReadOnlyList<StockBar> FilterByInterval(
            IReadOnlyList<StockBar> bars,
            TimeInterval interval)
        {
            if (bars == null || bars.Count == 0)
                return Array.Empty<StockBar>();

            var now = DateTimeOffset.UtcNow;
            DateTimeOffset cutoffDate = interval switch
            {
                TimeInterval.OneMonth => now.AddMonths(-1),
                TimeInterval.ThreeMonths => now.AddMonths(-3),
                TimeInterval.SixMonths => now.AddMonths(-6),
                TimeInterval.OneYear => now.AddYears(-1),
                TimeInterval.TwoYears => now.AddYears(-2),
                TimeInterval.FiveYears => now.AddYears(-5),
                TimeInterval.TenYears => now.AddYears(-10),
                TimeInterval.All => DateTimeOffset.MinValue,
                _ => now.AddYears(-1)
            };

            return bars.Where(b => b.Timestamp >= cutoffDate).ToList();
        }

        /// <summary>
        /// Returns a user-friendly label for the time interval
        /// </summary>
        public static string GetIntervalLabel(TimeInterval interval) => interval switch
        {
            TimeInterval.OneMonth => "1 Month",
            TimeInterval.ThreeMonths => "3 Months",
            TimeInterval.SixMonths => "6 Months",
            TimeInterval.OneYear => "1 Year",
            TimeInterval.TwoYears => "2 Years",
            TimeInterval.FiveYears => "5 Years",
            TimeInterval.TenYears => "10 Years",
            TimeInterval.All => "All Data",
            _ => "Unknown"
        };
    }
}
