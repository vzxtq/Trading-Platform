using Microsoft.Extensions.DependencyInjection;

namespace TradingEngine.MatchingEngine
{
    public sealed class MatchingEngineBuilder
    {
        public IServiceCollection Services { get; }

        public MatchingEngineBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
