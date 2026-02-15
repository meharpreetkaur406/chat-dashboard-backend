using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    // Send message to specific connected user
    public async Task SendMessageToUser(string receiverId, object message)
    {
        await Clients.User(receiverId)
            .SendAsync("ReceiveMessage", message);
    }
}