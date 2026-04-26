using TradingEngine.Domain.Entities;

namespace TradingEngine.Application.Interfaces.Accounts
{
    public interface IAccountRepository
    {
        Task<UserAccountDomain?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);
        Task AddAsync(
            UserAccountDomain account,
            CancellationToken cancellationToken);
        Task UpdateAsync(
            UserAccountDomain account,
            CancellationToken cancellationToken);
    }
}
