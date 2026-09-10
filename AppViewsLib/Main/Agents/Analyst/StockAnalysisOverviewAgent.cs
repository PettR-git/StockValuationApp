using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using StockValuationApp.Entities.Stocks;

namespace StockLib.Main.Agents.Analyst
{
    public class StockAnalysisOverviewAgent : StockAgentBase
    {
        public StockAnalysisOverviewAgent(Kernel? kernel = null) : base(kernel) { }

        public override string AgentName => "Stock Overview Agent";

        protected override string SystemPrompt =>       
            "You are an automated S&P 500 equity researcher. " +
            "Provide a concise, professional narrative covering the company's business model, valuation, and long-term prospects. " +
            "Detail what the bulls and bears think about its market position. Positives and negatives do not need to be balanced. " +
            "Return your response ONLY as a raw, single valid JSON object with NO markdown formatting, NO backticks, and NO extra commentary. " +
            "Keep the total text across all JSON fields under 20 sentences.\n\n" +
            "Use the following exact JSON structure:\n" +
            "{\n" +
            "  \"valuationSummary\": \"<Overview of the business model, valuation, and long-term prospects.>\",\n" +
            "  \"positives\": \"<Bull and core drivers thesis>\",\n" +
            "  \"riskFactor\": \"<Bear and key thesis vulnerabilities>\",\n" +
            "  \"overallRating\": \"<Bullish / Bearish / Neutral / Overweight / Underweight>\"\n" +
            "}";
        

        protected override string BuildUserPrompt(Stock stock)
        {
            string userPrompt =
                $"Analyze this asset:\n" +
                $"Company: {stock.Name} ({stock.Ticker})\n";

            return userPrompt;
        }

        public override string ToString()
        {
            var str = base.ToString();

            if (str == null)
                return string.Empty;

            return str;
        }
    }
}
