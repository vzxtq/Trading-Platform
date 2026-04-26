using Microsoft.EntityFrameworkCore;
using TradingEngine.Application.Features.Positions.Dtos;
using TradingEngine.Application.Interfaces.Positions;
using TradingEngine.Infrastructure.Persistence;

namespace TradingEngine.Infrastructure.Repositories.Positions;

public sealed class PositionReadRepository : IPositionReadRepository
{
    private readonly TradingDbContext _dbContext;

    public PositionReadRepository(TradingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<PositionDto>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var positions = await _dbContext.Positions
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .ToListAsync(ct);

        return positions.Select(p => new PositionDto
        {
            Symbol = p.Symbol.Value,
            Quantity = p.Quantity.Value,
            AveragePrice = p.AverageCost,
            UnrealizedPnL = 0m,
            LastUpdated = new DateTimeOffset(p.UpdatedAt ?? p.CreatedAt).ToUnixTimeMilliseconds()
        }).ToList();
    }
}
