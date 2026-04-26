using TradingEngine.Domain.Entities;

namespace TradingEngine.Application.Interfaces.Accounts;

public interface IUserIdentityRepository
{
    Task<UserIdentityDomain?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task AddAsync(UserIdentityDomain identity, CancellationToken cancellationToken);
}
