using MediatR;
using Microsoft.AspNetCore.Mvc;
using TradingEngine.Application.Features.Symbols.Queries;
using TradingEngineApi.Extensions;

namespace TradingEngine.Api.Controllers;

[Route("api/[controller]")]
public class SymbolsController(IMediator mediator) : ApiController(mediator)
{
    [HttpGet]
    public async Task<IActionResult> GetAllSymbols(CancellationToken cancellationToken)
    {
        var symbols = await _mediator.Send(new GetAllSymbolsQuery(), cancellationToken);
        return symbols.ToActionResult();
    }
}
