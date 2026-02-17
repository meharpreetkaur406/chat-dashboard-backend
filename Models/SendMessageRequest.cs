namespace ChatDashboard.Api.DTOs
{
    //coming from frontend
    public class SendMessageRequest
    {
        public string SenderId { get; set; }
        // NEW (multi user support)
        public List<string>? ReceiverIds { get; set; }

        // OLD SUPPORT (single chat still works)
        public string? ReceiverId { get; set; }
        public string MessageBody { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}