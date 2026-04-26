using System.Text.Json.Serialization;
using TradingEngine.Application.Common;
using TradingEngine.Application.Features.Trades.Dtos;
using TradingEngine.Application.Interfaces.Trades;

namespace TradingEngine.Application.Features.Trades.Queries;

public class GetTradeHistoryQuery : IQuery<Result<IReadOnlyList<TradeDto>>>
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    public string? Symbol { get; set; }
}

public sealed class GetTradeHistoryQueryHandler : IQueryHandler<GetTradeHistoryQuery, Result<IReadOnlyList<TradeDto>>>
{
    private readonly ITradeReadRepository _tradeReadRepository;

    public GetTradeHistoryQueryHandler(ITradeReadRepository tradeReadRepository)
    {
        _tradeReadRepository = tradeReadRepository ?? throw new ArgumentNullException(nameof(tradeReadRepository));
    }

    public async Task<Result<IReadOnlyList<TradeDto>>> Handle(GetTradeHistoryQuery request, CancellationToken cancellationToken)
    {
        var trades = await _tradeReadRepository.GetByUserIdAsync(request.UserId, request.Symbol, cancellationToken);
        return Result<IReadOnlyList<TradeDto>>.Success(trades);
    }
}
