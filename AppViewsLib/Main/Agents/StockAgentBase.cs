using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Main.Agents
{
    public abstract class StockAgentBase
    {
        protected readonly Kernel Kernel;

        public abstract string AgentName { get; }
        protected abstract string SystemPrompt { get; }
        protected abstract string BuildUserPrompt(Stock stock);

        protected StockAgentBase(Kernel? kernel = null, string modelId = "llama3.1:8b", string endpoint = "http://127.0.0.1:11434")
        {
            // Reuse an existing Kernel instance if passed, or build a local default
            if (kernel != null)
            {
                Kernel = kernel;
            }
            else
            {
                var builder = Kernel.CreateBuilder();
                builder.AddOllamaChatCompletion(
                    modelId: modelId,
                    endpoint: new Uri(endpoint)
                );
                Kernel = builder.Build();
            }
        }

        public virtual async Task<string> AnalyzeAsync(Stock stock, CancellationToken cancellationToken = default)
        {
            var executionSettings = new OllamaPromptExecutionSettings
            {
                Temperature = 0.2f,
                NumPredict = 1024,
                ExtensionData = new Dictionary<string, object>
                {
                    { "format", "json" }
                }
            };

            var promptTemplate = @"{{$systemPrompt}}

            {{$userPrompt}}";

            var args = new KernelArguments(executionSettings)
            {
                ["systemPrompt"] = SystemPrompt,
                ["userPrompt"] = BuildUserPrompt(stock)
            };

            var result = await Kernel.InvokePromptAsync(promptTemplate, args, cancellationToken: cancellationToken);
            return result.ToString().Trim();
        }
    }
}
