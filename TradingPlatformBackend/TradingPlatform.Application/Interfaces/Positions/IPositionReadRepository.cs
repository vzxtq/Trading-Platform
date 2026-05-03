using TradingEngine.Application.Features.Positions.Dtos;

namespace TradingEngine.Application.Interfaces.Positions;

public interface IPositionReadRepository
{
    Task<IReadOnlyList<PositionDto>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<PositionDto?> GetUserPositionForSymbolAsync(Guid userId, string symbol, CancellationToken ct);
}
