using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradingEngine.Application.Features.Accounts.Dtos;
using TradingEngine.Application.Interfaces.Accounts;
using TradingEngine.Infrastructure.Persistence;

namespace TradingEngine.Infrastructure.Repositories.Accounts
{
    public class AccountReadRepository : IAccountReadRepository
    {
        private readonly TradingDbContext _dbContext;

        public AccountReadRepository(TradingDbContext context)
        {
            _dbContext = context;
        }

        public async Task<AccountViewDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var account = await _dbContext.UserAccounts.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (account == null)
                return null;

            return new AccountViewDto
            {
                Id = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                FirstName = account.FirstName,
                LastName = account.LastName,
                Balance = account.Balance,
                ReservedBalance = account.ReservedBalance,
                AvailableBalance = account.AvailableBalance,
                LastLoginAt = account.LastLoginAt,
                IsActive = account.IsActive,
                CreatedAt = account.CreatedAt
            };
        }
    }
}
