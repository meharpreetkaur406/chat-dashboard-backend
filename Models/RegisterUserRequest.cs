namespace ChatDashboard.Api.DTOs
{
    public class RegisterUserRequest
    {
        public string? _Id { get; set; }
        public string? Type {get; set; }
        public string? Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
        public string RequestedRole { get; set; }
        public string? Status { get; set; }
        public string? CreatedAt { get; set; }

    }
}
