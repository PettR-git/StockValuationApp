using Newtonsoft.Json.Linq;
using StockPresentationLib.Utilities;
using StockValuationApp.Entities.Stocks;
using StockLib.Main.Entities.Stocks.Metrics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using OxyPlot;
using OxyPlot.Series;

namespace StockPresentationLib.ViewModel
{
    public class AnalysisVM : ViewModelBase
    {
        private Stock _stock;
        private string _valuationSummary;
        private string _positives;
        private string _riskFactor;
        private string _overallRating;
        private IReadOnlyList<StockBar>? _allBars;
        private IReadOnlyList<StockBar> _filteredBars;
        private TimeInterval _selectedInterval;
        private PlotModel _chartModel;
        private readonly StockManager? _stockManager;

        public AnalysisVM(StockManager? stockManager = null)
        {
            _stockManager = stockManager;
            _selectedInterval = TimeInterval.OneYear;
            _allBars = Array.Empty<StockBar>();
            _filteredBars = Array.Empty<StockBar>();
            InitializeChart();
        }

        private void InitializeChart()
        {
            _chartModel = new PlotModel
            {
                Title = "Stock Price History",
                Background = OxyColors.Transparent,
                TextColor = OxyColors.White,
                PlotAreaBorderColor = OxyColors.White
            };
        }

        public Stock Stock
        {
            get => _stock;
            set
            {
                if (_stock != value)
                {
                    if (_stock != null)
                    {
                        _stock.PropertyChanged -= OnStockPropertyChanged;
                    }

                    _stock = value;

                    if (_stock != null)
                    {
                        _stock.PropertyChanged += OnStockPropertyChanged;

                        // Load price data immediately when stock is selected
                        LoadPriceDataAsync();
                    }

                    OnPropertyChanged();
                    ParseAgentJson();
                    RefreshCandleData();
                }
            }
        }

        private void OnStockPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Stock.AgentAnalysisJson))
            {
                ParseAgentJson();
            }
        }

        public string ValuationSummary { get => _valuationSummary; set { _valuationSummary = value; OnPropertyChanged(); } }
        public string Positives { get => _positives; set { _positives = value; OnPropertyChanged(); } }
        public string RiskFactor { get => _riskFactor; set { _riskFactor = value; OnPropertyChanged(); } }
        public string OverallRating { get => _overallRating; set { _overallRating = value; OnPropertyChanged(); } }

        public IReadOnlyList<StockBar> FilteredBars
        {
            get => _filteredBars;
            set { _filteredBars = value; OnPropertyChanged(); }
        }

        public TimeInterval SelectedInterval
        {
            get => _selectedInterval;
            set
            {
                if (_selectedInterval != value)
                {
                    _selectedInterval = value;
                    OnPropertyChanged();
                    RefreshCandleData();
                }
            }
        }

        public List<TimeInterval> AvailableIntervals { get; } = 
            Enum.GetValues(typeof(TimeInterval)).Cast<TimeInterval>().ToList();

        public PlotModel ChartModel
        {
            get => _chartModel;
            set { _chartModel = value; OnPropertyChanged(); }
        }

        private void ParseAgentJson()
        {
            if (_stock == null || string.IsNullOrWhiteSpace(_stock.AgentAnalysisJson))
            {
                ResetFields("Select a stock with processed local AI insights.");
                return;
            }

            try
            {
                var jsonObject = JObject.Parse(_stock.AgentAnalysisJson);

                ValuationSummary = jsonObject["valuationSummary"]?.ToString() ?? "Not analyzed";
                Positives = jsonObject["positives"]?.ToString() ?? "Not analyzed";
                RiskFactor = jsonObject["riskFactor"]?.ToString() ?? "Not analyzed";
                OverallRating = jsonObject["overallRating"]?.ToString() ?? "N/A";
            }
            catch (Exception)
            {
                // Fallback rendering structure if JSON contains unstructured text or alternative formats
                ResetFields("Unable to parse structured JSON block.");
                ValuationSummary = _stock.AgentAnalysisJson; // Let the raw output serve as copy
            }
        }

        private void ResetFields(string defaultText)
        {
            ValuationSummary = defaultText;
            Positives = defaultText;
            RiskFactor = defaultText;
            OverallRating = "N/A";
        }

        /// <summary>
        /// Load price data for the current stock (called when stock is selected)
        /// </summary>
        private void LoadPriceDataAsync()
        {
            if (_stock == null)
            {
                return;
            }
            _allBars = _stock.WeeklyPrices;

            RefreshCandleData();
        }

        private void RefreshCandleData()
        {
            if (_allBars == null || _allBars.Count == 0)
            {
                _filteredBars = Array.Empty<StockBar>();
                UpdateCandleChart();
                return;
            }
            FilteredBars = CandleChartDataFilter.FilterByInterval(_allBars, _selectedInterval);
            UpdateCandleChart();
        }

        private void UpdateCandleChart()
        {
            _chartModel.Series.Clear();
            _chartModel.Axes.Clear();

            if (_filteredBars == null || _filteredBars.Count == 0)
            {
                _chartModel.Title = $"No data available ({_filteredBars?.Count ?? 0} bars)";
                _chartModel.InvalidatePlot(true);
                ChartModel = new PlotModel { Title = "No data available" };
                return;
            }

            // Create candlestick series
            var candleSeries = new CandleStickSeries
            {
                Title = "Price",
                Color = OxyColors.White,
                IncreasingColor = OxyColors.LimeGreen,
                DecreasingColor = OxyColors.Red
            };

            try
            {
                foreach (var bar in _filteredBars)
                {
                    var timeStamp = OxyPlot.Axes.DateTimeAxis.ToDouble(bar.Timestamp.DateTime);
                    var highLowItem = new HighLowItem
                    {
                        X = timeStamp,
                        High = (double)bar.High,
                        Low = (double)bar.Low,
                        Open = (double)bar.Open,
                        Close = (double)bar.Close
                    };
                    candleSeries.Items.Add(highLowItem);
                }
            }
            catch (Exception ex)
            {
                return;
            }

            _chartModel.Series.Add(candleSeries);

            // Setup axes
            var dateAxis = new OxyPlot.Axes.DateTimeAxis 
            { 
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                StringFormat = "yyyy-MM-dd",
                TextColor = OxyColors.White,
                TicklineColor = OxyColors.White,
                MajorGridlineStyle = OxyPlot.LineStyle.None
            };

            var valueAxis = new OxyPlot.Axes.LinearAxis 
            { 
                Position = OxyPlot.Axes.AxisPosition.Left,
                TextColor = OxyColors.White,
                TicklineColor = OxyColors.White,
                MajorGridlineStyle = OxyPlot.LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromArgb(50, 255, 255, 255)
            };

            _chartModel.Axes.Add(dateAxis);
            _chartModel.Axes.Add(valueAxis);

            _chartModel.Title = $"{_stock?.Ticker ?? "Stock"} Price - {_filteredBars.Count} candles";
            _chartModel.InvalidatePlot(true);

            // CRITICAL: Reassign to trigger binding update
            ChartModel = _chartModel;
        }
    }
}
