namespace ChatDashboard.Api.DTOs
{
    public class MessagesPerDayDto
    {
        public string DayName { get; set; }       // Mon, Tue, etc.
        public DateTime MessageDate { get; set; } // YYYY-MM-DD
        public int MessagesSent { get; set; }
    }
}