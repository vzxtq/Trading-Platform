using TradingEngine.Application.Common.Models;

namespace TradingEngine.Application.Features.Orders.Dtos;

public record OrderListResponseDto(
    PagedResult<OrderListDto> Orders,
    OrderSummaryDto Summary,
    SortingOptions? Sorting);
