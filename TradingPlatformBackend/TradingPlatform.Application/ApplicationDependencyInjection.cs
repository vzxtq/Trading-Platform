using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TradingEngine.MatchingEngine;
using TradingEngine.MatchingEngine.Handlers;

namespace TradingEngine.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationDependencyInjection).Assembly));

        services.AddMatchingEngine()
                .AddHandler<MarketDataExecutionResultHandler>();

        return services;
    }
}
