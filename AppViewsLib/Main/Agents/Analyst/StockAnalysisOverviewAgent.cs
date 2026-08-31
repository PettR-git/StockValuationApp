using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using StockValuationApp.Entities.Stocks;

namespace StockLib.Main.Agents.Analyst
{
    public class StockAnalysisOverviewAgent
    {
        private readonly Kernel _kernel;

        public StockAnalysisOverviewAgent()
        {
            var builder = Kernel.CreateBuilder();
            // Connects natively to your background Ollama endpoint
            builder.AddOllamaChatCompletion(
                modelId: "llama3.1:8b",
                endpoint: new Uri("http://127.0.0.1:11434")
            );
            _kernel = builder.Build();
        }

        public async Task<string> GenerateContextualAnalysisAsync(Stock stock)
        {
            string systemPrompt =
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

            string userPrompt =
                $"Analyze this asset:\n" +
                $"Company: {stock.Name} ({stock.Ticker})\n";


            var promptTemplate = @"{{$systemPrompt}}

            {{$userPrompt}}";

            var executionSettings = new OllamaPromptExecutionSettings
            {
                Temperature = 0.2f, // Keeps facts consistent and JSON strictly formatted
                NumPredict = 1024,  // Gives plenty of room to write detailed responses without truncating
                ExtensionData = new Dictionary<string, object>
                {
                    { "format", "json" }
                }
            };

            var args = new KernelArguments(executionSettings)
            {
                ["systemPrompt"] = systemPrompt,
                ["userPrompt"] = userPrompt
            };

            var result = await _kernel.InvokePromptAsync(promptTemplate, args);
            return result.ToString().Trim();
        }
    }
}
