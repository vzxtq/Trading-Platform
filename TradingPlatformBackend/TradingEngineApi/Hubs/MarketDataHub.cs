using Microsoft.AspNetCore.SignalR;

namespace TradingEngine.Api.Hubs
{
    public class MarketDataHub : Hub
    {
        public async Task JoinSymbol(string symbol)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, symbol);
        }

        public async Task LeaveSymbol(string symbol)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, symbol);
        }
    }
}
