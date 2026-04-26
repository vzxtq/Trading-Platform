using System.Text.Json.Serialization;
using TradingEngine.Application.Common;
using TradingEngine.Application.Features.Positions.Dtos;
using TradingEngine.Application.Interfaces.Positions;

namespace TradingEngine.Application.Features.Positions.Queries;

public class GetPositionsByUserIdQuery : IQuery<Result<IReadOnlyList<PositionDto>>>
{
    [JsonIgnore]
    public Guid UserId { get; set; }
}

public sealed class GetPositionsByUserIdQueryHandler : IQueryHandler<GetPositionsByUserIdQuery, Result<IReadOnlyList<PositionDto>>>
{
    private readonly IPositionReadRepository _positionReadRepository;

    public GetPositionsByUserIdQueryHandler(IPositionReadRepository positionReadRepository)
    {
        _positionReadRepository = positionReadRepository ?? throw new ArgumentNullException(nameof(positionReadRepository));
    }

    public async Task<Result<IReadOnlyList<PositionDto>>> Handle(GetPositionsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var positions = await _positionReadRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        return Result<IReadOnlyList<PositionDto>>.Success(positions);
    }
}
