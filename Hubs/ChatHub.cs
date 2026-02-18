using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatDashboard.Api.Hubs
{
    public class ChatHub : Hub
    {
        // 1️⃣ Method called by clients to send a message
        public async Task SendMessage(string receiverId, string user, string message)
        {
            // Broadcast the message to all connected clients
           await Clients.User(receiverId).SendAsync("ReceiveMessage", user, message);
        }

        // Optional: called when a client connects
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = Context.User?.FindFirst("username")?.Value;

            await base.OnConnectedAsync();
        }

        // Optional: called when a client disconnects
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            // You can log disconnection here
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        }
    }
}