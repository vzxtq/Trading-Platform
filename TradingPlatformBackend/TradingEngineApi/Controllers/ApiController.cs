using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TradingEngine.Api.Controllers;

[ApiController]
public abstract class ApiController(IMediator mediator) : ControllerBase
{
    protected readonly IMediator _mediator = mediator;
}
