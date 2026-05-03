using TradingEngine.Domain.Entities;

namespace TradingEngine.Application.Interfaces.Accounts;

public interface IUserIdentityRepository
{
    Task<UserIdentityDomain?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<UserIdentityDomain?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<UserIdentityDomain?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(UserIdentityDomain identity, CancellationToken cancellationToken);
    Task UpdateAsync(UserIdentityDomain identity, CancellationToken cancellationToken);
}
