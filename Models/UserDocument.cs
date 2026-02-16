using System.Text.Json.Serialization;

namespace ChatDashboard.Api.Models
{
    public class UserDocument
    {
        [JsonPropertyName("_id")]      
        public string _id { get; set; }

        [JsonPropertyName("_rev")]
        public string _rev { get; set; }
        public string type { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        public string? role { get; set; }
        public string? requestedRole { get; set; }

        public string status { get; set; }
        public string createdAt { get; set; }
    }
}
