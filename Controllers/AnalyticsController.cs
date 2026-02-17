using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using ChatDashboard.Api.Services;
using ChatDashboard.Api.DTOs;

namespace ChatDashboard.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AnalyticsService _analyticsService;

        public  AnalyticsController(AnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("messages/sent-per-day")]
        public async Task<List<MessagesPerDayDto>> GetNumberOfMessagesSentPerDay()
        {
            return await _analyticsService.GetMessagesPerDayAsync();
        }
    }
}