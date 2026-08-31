using System;
using System.Globalization;
using System.Windows.Data;
using StockPresentationLib.Utilities;

namespace StockPresentationLib.Utilities
{
    public class IntervalToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeInterval interval)
            {
                return CandleChartDataFilter.GetIntervalLabel(interval);
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
