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
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("pending-users")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var result = await _adminService.GetAllPendingUsers();

            return Content(result, "application/json");
        }

        [Authorize(Roles = "admin")]
        [HttpPost("approve-user/{id}")]
        public async Task<IActionResult> ApproveUser(string id, [FromBody] ApproveUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Role))
                return BadRequest("Role required");
            await _adminService.ApproveUserAsync(id, request.Role.ToLower());
            return Ok("User approved");
        }
    }
}