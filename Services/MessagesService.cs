using ChatDashboard.Api.DTOs;
using ChatDashboard.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

public class MessagesService
{
    private readonly AppDbContext _context;

    public MessagesService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MessageWithTargetDto>> GetMessagesReceivedByUserAsync(string userId)
    {
        var messages = await (from m in _context.Messages
                                  join mt in _context.MessageTargets
                                  on m.MessageId equals mt.MessageId
                                  where mt.TargetId == userId
                                  select new MessageWithTargetDto
                                  {
                                    MessageId = m.MessageId,
                                    SenderId = m.SenderId,
                                    MessageBody = m.MessageBody,
                                    CreatedAt = m.CreatedAt,
                                    TargetId = mt.TargetId
                                })
                                 .ToListAsync();
        
        return messages;
    }

    public async Task<List<MessageWithTargetDto>> GetMessagesBySenderAsync(string senderId)
    {
        return await (from m in _context.Messages
                        join mt in _context.MessageTargets
                        on m.MessageId equals mt.MessageId
                        where m.SenderId == senderId
                        select new MessageWithTargetDto
                        {
                            MessageId = m.MessageId,
                            SenderId = m.SenderId,
                            MessageBody = m.MessageBody,
                            CreatedAt = m.CreatedAt,
                            TargetId = mt.TargetId
                        })
                        .OrderByDescending(x => x.CreatedAt)
                        .ToListAsync();
    }
}
