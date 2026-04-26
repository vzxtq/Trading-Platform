using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TradingEngine.MatchingEngine.Services;

namespace TradingEngine.MatchingEngine.Services.Background;

public sealed class MatchingEngineBackgroundService : BackgroundService
{
    private readonly MatchingEngineHost _host;
    private readonly ILogger<MatchingEngineBackgroundService> _logger;

    public MatchingEngineBackgroundService(
        MatchingEngineHost host,
        ILogger<MatchingEngineBackgroundService> logger)
    {
        _host = host;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Matching Engine starting");
        try
        {
            await _host.RunAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Matching Engine fatal error");
            throw;
        }
        finally
        {
            _logger.LogInformation("Matching Engine stopped");
        }
    }
}
