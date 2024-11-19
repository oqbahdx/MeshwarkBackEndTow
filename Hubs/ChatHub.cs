using Microsoft.AspNetCore.SignalR;

namespace Meshwark.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> userConnectionMap = new Dictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                userConnectionMap[userId] = Context.ConnectionId;
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId) && userConnectionMap.ContainsKey(userId))
            {
                userConnectionMap.Remove(userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToUser(string userId, string message)
        {
            if (userConnectionMap.TryGetValue(userId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
            }
        }
    }
}

        
