using System;

namespace ChatDashboard.Api.DTOs
{
    public class Message
    {
        public int MessageId { get; set; }
        public string SenderId { get; set; }
        public string EncryptedMessage { get; set; }
        public string EncryptedKey { get; set; }
        public string HashMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
