using System;

namespace ChatDashboard.Api.DTOs
{
    public class Message
    {
        public int MessageId { get; set; }
        public string SenderId { get; set; }
        public string MessageBody { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
