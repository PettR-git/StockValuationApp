using StockLib.Main.Entities.Stocks;
using StockLib.Main.Entities.Stocks.Metrics;
using StockValuationApp.Entities.Stocks.Metrics;
using StockValuationApp.Entities.Stocks.Metrics.Earnings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StockValuationApp.Entities.Stocks
{
    /// <summary>
    /// A stock's metric and financial data.
    /// </summary>
    [Serializable]
    public class Stock : INotifyPropertyChanged
    {
        private string? _agentAnalysisJson;
        public EventHandler<MetricEventArgs> MetricsGiven;
        public EventHandler<WeeklyStockPricesEventArgs> WeeklyStockPricesGiven;

        public Stock()
        {
            YearlyFinancials = new List<YearlyFinancials>();
            StockScore = new StockScore();
            LastUpdated = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Ticker { get; set; } = string.Empty;
        public decimal LastPrice { get; set; }

        public IReadOnlyList<StockBar>? WeeklyPrices { get; set; } = Array.Empty<StockBar>();
        public DateTime LastUpdated { get; set; }

        // Financials for a specific year
        public List<YearlyFinancials> YearlyFinancials { get; set; }

        // Score
        public StockScore? StockScore { get; set; }

        //Agent analysis JSON string, which can be updated by the agent and will trigger UI updates when changed
        public string? AgentAnalysisJson
        {
            get => _agentAnalysisJson;
            set
            {
                if (_agentAnalysisJson != value)
                {
                    _agentAnalysisJson = value;
                    OnPropertyChanged(); 
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public override string ToString()
        {
            return $"Stock: {Name}, ${Ticker.ToUpperInvariant()}";
        }
    }
}
