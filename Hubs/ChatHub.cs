using Microsoft.AspNetCore.SignalR;
using auth.Controllers;
namespace auth.Hubs
{

    public class ChatHub : Hub
    {
        public async Task SendMessage(ChatMessage message)
        {
            await Clients.User(message.SenderId).SendAsync("ReceiveMessage", message);
            await Clients.User(message.ReceiverId).SendAsync("ReceiveMessage", message);
        }
    }



    /*
    public class ChatHub : Hub
    {
        public async Task SendMessage(string senderId, string receiverId, string content)
        {
            // Send to specific users
            await Clients.User(senderId).SendAsync("ReceiveMessage", senderId, receiverId, content);
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, receiverId, content);
        }
    }

    */
}


