using Microsoft.EntityFrameworkCore;
using TradingEngine.Application.Features.Orders.Repositories;
using TradingEngine.Infrastructure.Persistence;
using TradingEngine.Domain.Entities;
using TradingEngine.Application.Interfaces.Orders;

namespace TradingEngine.Infrastructure.Repositories.Orders;

public sealed class OrderRepository : IOrderRepository
{
    private readonly TradingDbContext _dbContext;

    public OrderRepository(TradingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OrderDomain order, CancellationToken cancellationToken)
    {
        await _dbContext.Orders.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(OrderDomain order, CancellationToken cancellationToken)
    {
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<OrderDomain?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public async Task<IEnumerable<OrderDomain>> GetUserOrdersAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
