using Microsoft.AspNetCore.SignalR;

namespace zSubscription.MVCWebApp.KhoiNBD.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
