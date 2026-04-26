using TradingEngine.MatchingEngine.Models;

namespace TradingEngine.MatchingEngine.Abstractions
{
    public interface IExecutionResultHandler
    {
        Task HandleAsync(ExecutionResult result, CancellationToken cancellationToken);
    }
}
