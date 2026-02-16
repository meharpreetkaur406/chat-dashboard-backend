using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using ChatDashboard.Api.Data;
using ChatDashboard.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace ChatDashboard.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly UserService _userService;

        public UsersController(AppDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
    }
}