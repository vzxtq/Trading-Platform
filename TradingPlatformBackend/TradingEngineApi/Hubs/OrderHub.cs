using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MediatR;
using TradingEngine.Application.Features.Accounts.Commands;

namespace TradingEngine.Api.Hubs
{
    [Authorize]
    public class OrderHub : Hub
    {
        private readonly IMediator _mediator;

        public OrderHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task OnConnectedAsync()
        {
            var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out var userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                await _mediator.Send(new SetUserActiveCommand(userId, true));
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out var userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                await _mediator.Send(new SetUserActiveCommand(userId, false));
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
