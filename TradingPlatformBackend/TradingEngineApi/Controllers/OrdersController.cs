using MediatR;
using Microsoft.AspNetCore.Mvc;
using TradingEngine.Application.Features.Orders.Commands;
using TradingEngine.Application.Features.Orders.Queries;
using TradingEngineApi.Extensions;
using TradingEngine.Application.Interfaces;

namespace TradingEngine.Api.Controllers;

[Route("api/[controller]")]
public class OrdersController : ApiController
{
    private readonly IUserResolverService _userResolverService;

    public OrdersController(IMediator mediator, IUserResolverService userResolverService) : base(mediator)
    {
        _userResolverService = userResolverService;
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(
        [FromBody] PlaceOrderCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(
        Guid id,
        CancelOrderCommand command,
        CancellationToken ct)
    {
        command.OrderId = id;
        command.UserId = _userResolverService.GetUserId();

        var result = await _mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetOrderByIdQuery { OrderId = id };
        var result = await _mediator.Send(query, ct);

        return result.ToActionResult();
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId, CancellationToken ct)
    {
        var query = new GetUserOrdersQuery { UserId = userId };
        var result = await _mediator.Send(query, ct);

        return result.ToActionResult();
    }

    [HttpGet("book/{symbol}")]
    public async Task<IActionResult> GetOrderBook(string symbol, CancellationToken ct)
    {
        var query = new GetOrderBookQuery { Symbol = symbol };
        var result = await _mediator.Send(query, ct);

        return result.ToActionResult();
    }
}
