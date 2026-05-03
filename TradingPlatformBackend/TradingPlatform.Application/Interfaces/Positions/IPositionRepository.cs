using TradingEngine.Domain.Entities;

namespace TradingEngine.Application.Interfaces.Positions
{
    public interface IPositionRepository
    {
        Task<PositionDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<PositionDomain?> GetUserPositionForSymbolAsync(Guid userId, string symbol, CancellationToken cancellationToken);
        Task UpdateAsync(PositionDomain position, CancellationToken cancellationToken);
        Task AddAsync(PositionDomain position, CancellationToken cancellationToken);
    }
}
