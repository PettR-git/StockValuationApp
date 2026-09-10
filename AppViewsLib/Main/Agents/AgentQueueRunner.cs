using StockValuationApp.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Main.Agents
{
    public class AgentQueueRunner
    {
        private readonly List<StockAgentBase> _agents;

        public AgentQueueRunner(IEnumerable<StockAgentBase> agents)
        {
            _agents = agents.ToList();
        }

        /// <summary>
        /// Executes all agents sequentially in a queue.
        /// </summary>
        public async Task RunQueueAsync(Stock stock, Action<string, string>? onAgentCompleted = null, CancellationToken cancellationToken = default)
        {
            foreach (var agent in _agents)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string result = await agent.AnalyzeAsync(stock, cancellationToken);
                onAgentCompleted?.Invoke(agent.AgentName, result);
            }
        }
    }
}
