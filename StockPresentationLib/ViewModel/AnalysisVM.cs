using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using OxyPlot;
using OxyPlot.Series;
using StockLib.Main.Agents.Analyst;
using StockLib.Main.Agents.Analyst.Technical;
using StockLib.Main.Entities.Stocks.Metrics;
using StockPresentationLib.Utilities;
using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace StockPresentationLib.ViewModel
{
    public partial class AnalysisVM : ViewModelBase
    {
        private Stock? _stock;
        private string? _valuationSummary;
        private string? _positives;
        private string? _riskFactor;
        private string? _overallRating;

        // Technical Analysis Fields
        private string? _trendOutlook;
        private string? _keySupportResistance;
        private string? _mediumTermVerdict;
        private string? _longTermVerdict;
        private bool _isTechnicalAnalysisVisible;

        private IReadOnlyList<StockBar>? _allBars;
        private IReadOnlyList<StockBar> _filteredBars;
        private TimeInterval _selectedInterval;
        private PlotModel? _chartModel;
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

        public Stock? Stock
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
                    ParseOverviewJson();
                    ParseTechnicalAnalysisJson(); // Enabled
                    RefreshCandleData();
                }
            }
        }

        private void OnStockPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Stock.OverviewAgentJson))
            {
                ParseOverviewJson();
            }
            else if (e.PropertyName == nameof(Stock.TechnicalAnalysisAgentJson))
            {
                ParseTechnicalAnalysisJson();
            }
        }

        // Fundamentals Properties
        public string? ValuationSummary { get => _valuationSummary; set { _valuationSummary = value; OnPropertyChanged(); } }
        public string? Positives { get => _positives; set { _positives = value; OnPropertyChanged(); } }
        public string? RiskFactor { get => _riskFactor; set { _riskFactor = value; OnPropertyChanged(); } }
        public string? OverallRating { get => _overallRating; set { _overallRating = value; OnPropertyChanged(); } }

        // Technical Analysis Properties
        public string? TrendOutlook { get => _trendOutlook; set { _trendOutlook = value; OnPropertyChanged(); } }
        public string? KeySupportResistance { get => _keySupportResistance; set { _keySupportResistance = value; OnPropertyChanged(); } }
        public string? MediumTermVerdict { get => _mediumTermVerdict; set { _mediumTermVerdict = value; OnPropertyChanged(); } }
        public string? LongTermVerdict { get => _longTermVerdict; set { _longTermVerdict = value; OnPropertyChanged(); } }

        public bool IsTechnicalAnalysisVisible
        {
            get => _isTechnicalAnalysisVisible;
            set { _isTechnicalAnalysisVisible = value; OnPropertyChanged(); }
        }

        [RelayCommand]
        private async Task OverviewAsync()
        {
            if (_stockManager != null && _stock != null)
            {
                await _stockManager.RunAgentAnalysisAsync(_stock, new StockAnalysisOverviewAgent());
                await _stockManager.SaveAllAsync();
            }
        }

        [RelayCommand]
        private async Task TechnicalAnalysisAsync()
        {
            if (_stockManager != null && _stock != null)
            {
                await _stockManager.RunAgentAnalysisAsync(_stock, new TechnicalAnalysisAgent());
                await _stockManager.SaveAllAsync();
            }
        }

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

        public PlotModel? ChartModel
        {
            get => _chartModel;
            set { _chartModel = value; OnPropertyChanged(); }
        }

        private void ParseOverviewJson()
        {
            if (_stock == null || string.IsNullOrWhiteSpace(_stock.OverviewAgentJson))
            {
                ResetFields("Select a stock with processed local AI insights.");
                return;
            }

            try
            {
                var jsonObject = JObject.Parse(_stock.OverviewAgentJson);

                ValuationSummary = jsonObject["valuationSummary"]?.ToString() ?? "Not analyzed";
                Positives = jsonObject["positives"]?.ToString() ?? "Not analyzed";
                RiskFactor = jsonObject["riskFactor"]?.ToString() ?? "Not analyzed";
                OverallRating = jsonObject["overallRating"]?.ToString() ?? "N/A";
            }
            catch (Exception)
            {
                ResetFields("Unable to parse structured JSON block.");
                ValuationSummary = _stock.OverviewAgentJson;
            }
        }

        private void ParseTechnicalAnalysisJson()
        {
            if (_stock == null || string.IsNullOrWhiteSpace(_stock.TechnicalAnalysisAgentJson))
            {
                IsTechnicalAnalysisVisible = false;
                ResetTechnicalFields();
                return;
            }

            try
            {
                var jsonObject = JObject.Parse(_stock.TechnicalAnalysisAgentJson);

                TrendOutlook = jsonObject["trendOutlook"]?.ToString() ?? "Not analyzed";
                KeySupportResistance = jsonObject["keySupportResistance"]?.ToString() ?? "Not analyzed";
                MediumTermVerdict = jsonObject["mediumTermVerdict"]?.ToString() ?? "N/A";
                LongTermVerdict = jsonObject["longTermVerdict"]?.ToString() ?? "N/A";

                IsTechnicalAnalysisVisible = true;
            }
            catch (Exception)
            {
                IsTechnicalAnalysisVisible = false;
                ResetTechnicalFields();
            }
        }

        private void ResetFields(string defaultText)
        {
            ValuationSummary = defaultText;
            Positives = defaultText;
            RiskFactor = defaultText;
            OverallRating = "N/A";
        }

        private void ResetTechnicalFields()
        {
            TrendOutlook = null;
            KeySupportResistance = null;
            MediumTermVerdict = null;
            LongTermVerdict = null;
        }

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
            if (_chartModel == null)
            {
                return;
            }

            _chartModel.Series.Clear();
            _chartModel.Axes.Clear();

            if (_filteredBars == null || _filteredBars.Count == 0)
            {
                _chartModel.Title = $"No data available ({_filteredBars?.Count ?? 0} bars)";
                _chartModel?.InvalidatePlot(true);
                ChartModel = new PlotModel { Title = "No data available" };
                return;
            }

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
            catch (Exception)
            {
                return;
            }

            _chartModel.Series.Add(candleSeries);

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

            ChartModel = _chartModel;
        }
    }
}