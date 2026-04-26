using TradingEngine.Application.Features.Accounts.Dtos;

namespace TradingEngine.Application.Interfaces.Accounts
{
    public interface IAccountReadRepository
    {
        Task<AccountViewDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
