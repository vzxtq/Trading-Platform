using TradingEngine.Application.Common;
using TradingEngine.Application.Interfaces.Symbols;

namespace TradingEngine.Application.Features.Symbols.Queries;

public record GetAllSymbolsQuery : IQuery<Result<List<string>>>;

public sealed class GetAllSymbolsQueryHandler : IQueryHandler<GetAllSymbolsQuery, Result<List<string>>>
{
    private readonly ISymbolReadRepository _readRepository;

    public GetAllSymbolsQueryHandler(ISymbolReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<Result<List<string>>> Handle(GetAllSymbolsQuery request, CancellationToken cancellationToken)
    {
        var result = await _readRepository.GetAllSymbolsAsync();

        return Result<List<string>>.Success(result);
    }
}
