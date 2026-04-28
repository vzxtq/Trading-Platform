using Microsoft.EntityFrameworkCore;
using TradingEngine.Application.Features.Trades.Dtos;
using TradingEngine.Application.Interfaces.Trades;
using TradingEngine.Domain.Enums;
using TradingEngine.Infrastructure.Persistence;

namespace TradingEngine.Infrastructure.Repositories.Trades;

public sealed class TradeReadRepository : ITradeReadRepository
{
    private readonly TradingDbContext _dbContext;

    public TradeReadRepository(TradingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<TradeDto>> GetByUserIdAsync(Guid userId, string? symbol, CancellationToken ct)
    {
        var query = _dbContext.Trades
            .AsNoTracking()
            .Where(t => t.BuyerId == userId || t.SellerId == userId);

        if (!string.IsNullOrWhiteSpace(symbol))
        {
            query = query.Where(t => t.Symbol.Value == symbol);
        }

        var trades = await query
            .OrderByDescending(t => t.ExecutedAt)
            .ToListAsync(ct);

        return trades.Select(t => new TradeDto
        {
            TradeId = t.Id,
            Symbol = t.Symbol.Value,
            Price = t.Price.Value,
            Quantity = t.Quantity.Value,
            Side = t.BuyerId == userId ? OrderSide.Buy : OrderSide.Sell,
            ExecutedAt = t.ExecutedAt.ToUnixTimeMs()
        }).ToList();
    }
}
