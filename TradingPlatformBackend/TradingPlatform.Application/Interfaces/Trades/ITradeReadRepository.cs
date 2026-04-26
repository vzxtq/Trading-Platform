using TradingEngine.Application.Features.Trades.Dtos;

namespace TradingEngine.Application.Interfaces.Trades;

public interface ITradeReadRepository
{
    Task<IReadOnlyList<TradeDto>> GetByUserIdAsync(Guid userId, string? symbol, CancellationToken ct);
}
