using TradingEngine.Application.Common.Models;
using TradingEngine.Application.Features.Trades.Dtos;

namespace TradingEngine.Application.Interfaces.Trades;

public interface ITradeReadRepository
{
    Task<PagedResult<TradeDto>> GetByUserIdAsync(
        Guid userId,
        TradeFilterDto filter,
        PaginatedQuery pagination,
        CancellationToken ct);

    /// <summary>
    /// Returns the total notional (price * quantity) already committed for a buy order
    /// across all trades persisted so far. Used to compute the remaining reserved amount
    /// to release when cancelling a partially-filled buy order.
    /// </summary>
    Task<decimal> GetTotalSpentOnBuyOrderAsync(Guid orderId, CancellationToken ct);
}
