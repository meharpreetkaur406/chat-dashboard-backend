using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatDashboard.Api.Data;
using ChatDashboard.Api.DTOs;
using ChatDashboard.Api.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace ChatDashboard.Api.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly MessagesService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;


        public MessagesController(AppDbContext context, MessagesService messageService)
        {
            _context = context;
            _messageService = messageService;
        }

        [Authorize]
        [HttpGet("received/{userId}")]
        public async Task<IActionResult> GetMessagesReceivedByUser(string userId)
        {
            var messages = await _messageService.GetMessagesReceivedByUserAsync(userId);
            
            if (messages == null || messages.Count == 0)
            {
                return NotFound($"No messages found for user {userId}");
            }

            return Ok(messages);
        }

        [Authorize]
        [HttpGet("sent/{userId}")]
        public async Task<IActionResult> GetMessagesSentByUser(string userId)
        {
            var messages = await _messageService.GetMessagesBySenderAsync(userId);
            
            if (messages == null || messages.Count == 0)
            {
                return NotFound($"No messages found for user {userId}");
            }

            return Ok(messages);
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<Dictionary<string, List<MessageWithTargetDto>>> GetAllMessagesGroupedForUserAsync(string userId)
        {
            var messages = await _context.Messages
                .Join(_context.MessageTargets,
                    m => m.MessageId,
                    mt => mt.MessageId,
                    (m, mt) => new { m, mt })
                .Where(x => x.m.SenderId == userId || x.mt.TargetId == userId)
                .Select(x => new MessageWithTargetDto
                {
                    MessageId = x.m.MessageId,
                    SenderId = x.m.SenderId,
                    MessageBody = x.m.MessageBody,
                    CreatedAt = x.m.CreatedAt,
                    TargetId = x.mt.TargetId
                })
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();

            // Group by conversation key (SenderId + TargetId)
            var grouped = messages
                .GroupBy(x =>
                {
                    // Create a consistent key so that A->B and B->A are in the same group
                    var ids = new[] { x.SenderId, x.TargetId }.OrderBy(id => id).ToArray();
                    return $"{ids[0]}/{ids[1]}";
                })
                .ToDictionary(g => g.Key, g => g.ToList());

            return grouped;
        }

        [Authorize]
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var receivers = new List<string>();

            if (request.ReceiverIds != null && request.ReceiverIds.Any())
                receivers = request.ReceiverIds;
            else if (!string.IsNullOrEmpty(request.ReceiverId))
                receivers.Add(request.ReceiverId);
            else
                return BadRequest("No receiver provided");

            Console.WriteLine("request" ,request);
            var message = new Message
            {
                SenderId = request.SenderId,
                MessageBody = request.MessageBody,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var targets = receivers.Select(r => new MessageTarget
            {
                MessageId = message.MessageId,
                TargetId = r
            });

            _context.MessageTargets.AddRange(targets);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
