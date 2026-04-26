using TradingEngine.MatchingEngine.Models;

namespace TradingEngine.MatchingEngine.Abstractions
{
    public interface IExecutionResultDispatcher
    {
        Task DispatchAsync(ExecutionResult result, CancellationToken cancellationToken);
    }
}
