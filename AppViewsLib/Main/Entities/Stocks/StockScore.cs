using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Main.Entities.Stocks
{
    public class StockScore
    {
        private double valuationScore;
        private double moatScore;
        private double marketScore;
        public StockScore() 
        {
            valuationScore = 0;
            moatScore = 0;
            marketScore = 0;
        }
        
        //Valuation scores
        public double RevGrowthScore {  get; set; }
        public double EpsGrowthScore { get; set; }
        public double EvFcfScore {  get; set; }
        public double RoeRoicScore {  get; set; }
        public double EvEbitScore {  get; set; }

        //Moat scores
        public double NetworkEffectScore { get; set; } = 5;
        public double CostAdvScore { get; set; } = 4.5;
        public double SwitchCostScore { get; set; } = 4;
        public double ScalabilityScore { get; set; } = 3;
        public double IntangAssetScore { get; set; } = 4.75;

        //Underlying Parameter scores
        public double SectorGrowthScore { get; set; } = 3;
        public double NonDisruptiveScore { get; set; } = 1;
        public double ConsensusScore { get; set; } = 2.5;
        public double MarginExpScore { get; set; } = 1.5;
        public double MarketVolatilityScore { get; set; } = 2.5;

        //Get total score
        public double GetValuationScore
        {
            get 
            { 
                valuationScore = Math.Round(2*(RevGrowthScore + EpsGrowthScore + EvFcfScore + RoeRoicScore + EvEbitScore) / 5, 1);
                return valuationScore;
            }
        }
        public double GetMoatScore { 
            get
            {
                moatScore = Math.Round(2*(SwitchCostScore + CostAdvScore + IntangAssetScore + NetworkEffectScore + ScalabilityScore) / 5, 1);
                return moatScore;
            }
        }
        public double GetMarketScore 
        {
            get
            {
                marketScore = Math.Round(2*(SectorGrowthScore + MarginExpScore + MarketVolatilityScore + ConsensusScore + NonDisruptiveScore) / 5, 1);
                return marketScore;
            }
        }
    }
}
