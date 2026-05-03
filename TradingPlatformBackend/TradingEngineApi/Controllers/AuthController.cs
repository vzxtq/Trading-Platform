using MediatR;
using Microsoft.AspNetCore.Mvc;
using TradingEngine.Application.Features.Accounts.Commands;
using TradingEngineApi.Extensions;

namespace TradingEngine.Api.Controllers;

[Route("api/[controller]")]
public class AuthController : ApiController
{
    public AuthController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result.ToActionResult();
    }
}
