using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatDashboard.Api.Services   // match your project namespace
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("username")?.Value;
        }
    }
}
