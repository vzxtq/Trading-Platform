using TradingEngine.Application.Common.Models;
using TradingEngine.Application.Features.Orders.Dtos;
using TradingEngine.Domain.Entities;
using TradingEngine.Domain.Enums;
using TradingEngine.Domain.ValueObjects;

namespace TradingEngine.Application.Interfaces.Orders;

public interface IOrderReadRepository
{
    Task<PagedResult<OrderListDto>> GetOrdersAsync(
        Guid userId,
        OrderFilterDto filter,
        PaginatedQuery pagination,
        CancellationToken cancellationToken);

    Task<OrderSummary> GetOrderSummaryAsync(Guid userId, CancellationToken cancellationToken);

    Task<OrderDomain?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);
}
