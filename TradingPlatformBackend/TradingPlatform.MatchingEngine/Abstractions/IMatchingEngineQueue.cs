using TradingEngine.MatchingEngine.Commands;

namespace TradingEngine.MatchingEngine.Abstractions
{
    public interface IMatchingEngineQueue
    {
        ValueTask EnqueueAsync(MatchingEngineCommand command, CancellationToken cancellationToken);
    }
}
