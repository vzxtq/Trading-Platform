using TradingEngine.Domain.Entities;

namespace TradingEngine.Domain.Repositories;

/// <summary>
/// Repository interface for UserAccount aggregate root.
/// </summary>
public interface IUserAccountRepository
{
    Task<UserAccountDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<UserAccountDomain?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task AddAsync(UserAccountDomain account, CancellationToken cancellationToken = default);

    Task UpdateAsync(UserAccountDomain account, CancellationToken cancellationToken = default);

    Task<IEnumerable<UserAccountDomain>> GetAllAsync(CancellationToken cancellationToken = default);
}