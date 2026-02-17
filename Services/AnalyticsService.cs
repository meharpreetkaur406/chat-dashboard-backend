using System.Net.Http.Headers;
using System.Text;
using ChatDashboard.Api.DTOs;
using System.Text.Json;
using ChatDashboard.Api.Data;
using ChatDashboard.Api.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ChatDashboard.Api.Services
{
    public class AnalyticsService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        public AnalyticsService(HttpClient httpClient, AppDbContext context)
        {
            _httpClient = httpClient;
            var byteArray = Encoding.ASCII.GetBytes("mehar:Rbh@123");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            
            _context = context;
        }

        public async Task<List<MessagesPerDayDto>> GetMessagesPerDayAsync()
        {
            // Use UTC dates
            DateTime todayUtc = DateTime.UtcNow.Date;
            DateTime startDateUtc = todayUtc.AddDays(-6); // last 7 days including today

             // Step 1: Fetch data from DB using fully translatable expressions
            var rawData = await _context.Messages
                .Where(m => m.CreatedAt >= startDateUtc && m.CreatedAt < todayUtc.AddDays(1)) // range filter
                .GroupBy(m => new { m.CreatedAt.Year, m.CreatedAt.Month, m.CreatedAt.Day })
                .Select(g => new MessagesRawDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Day = g.Key.Day,
                    Count = g.Count()
                })
                .ToListAsync();


            var messagesPerDay = rawData
                .Select(x => new MessagesPerDayDto
                {
                    MessageDate = new DateTime(x.Year, x.Month, x.Day, 0, 0, 0, DateTimeKind.Utc),
                    DayName = new DateTime(x.Year, x.Month, x.Day, 0, 0, 0, DateTimeKind.Utc).ToString("ddd"),
                    MessagesSent = x.Count
                })
                .OrderBy(x => x.MessageDate)
                .ToList(); // LINQ-to-Objects


            return messagesPerDay ;
        }
    }
}