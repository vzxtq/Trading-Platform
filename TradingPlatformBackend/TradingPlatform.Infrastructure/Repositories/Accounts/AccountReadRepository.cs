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
                Name = account.FullName,
                Balance = account.Balance,
                ReservedBalance = account.ReservedBalance,
                LastLoginAt = account.LastLoginAt,
                IsActive = account.IsActive,
                CreatedAt = account.CreatedAt
            };
        }
    }
}
