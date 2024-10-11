using Database.EntityModels;
using Microsoft.AspNetCore.SignalR;

namespace WebPushNotificationApp.Mvc.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(Message message)
        {
            await Clients.All.SendAsync("receiveMessage", message);
            
        }
    }
}