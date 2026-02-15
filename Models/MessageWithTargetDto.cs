namespace ChatDashboard.Api.DTOs
{
    public class MessageWithTargetDto
    {
        public int MessageId { get; set; }
        public string SenderId { get; set; }
        public string MessageBody { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TargetId { get; set; }
    }
}
