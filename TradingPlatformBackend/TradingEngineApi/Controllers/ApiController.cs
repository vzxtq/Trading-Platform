using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TradingEngine.Api.Controllers;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected readonly IMediator _mediator;

    protected ApiController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
