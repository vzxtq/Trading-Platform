using Microsoft.EntityFrameworkCore;
using TradingEngine.Application.Interfaces.Accounts;
using TradingEngine.Infrastructure.Persistence;
using TradingEngine.Domain.Entities;

namespace TradingEngine.Infrastructure.Repositories.Accounts;

public sealed class UserIdentityRepository : IUserIdentityRepository
{
    private readonly TradingDbContext _dbContext;

    public UserIdentityRepository(TradingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserIdentityDomain?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.UserIdentities
            .Join(_dbContext.UserAccounts, identity => identity.UserId, account => account.Id, (identity, account) => new { identity, account })
            .Where(x => x.account.Email == email.ToLower())
            .Select(x => x.identity)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(UserIdentityDomain identity, CancellationToken cancellationToken)
    {
        await _dbContext.UserIdentities.AddAsync(identity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
