using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using ChatDashboard.Api.DTOs;
using ChatDashboard.Api.Services;

namespace ChatDashboard.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserRegisterService _userRegisterService;
        private readonly UserLoginService _userLoginService;

        public AuthController(UserRegisterService userRegisterService, UserLoginService userLoginService)
        {
            _userRegisterService = userRegisterService;
            _userLoginService = userLoginService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {   
            if ( string.IsNullOrWhiteSpace(request.Type) ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.Role))
            {
                return BadRequest("All fields are required.");
            }

            var userDoc = new
            {
                _id = Guid.NewGuid().ToString(),
                type = request?.Type,    
                username = request?.Username,
                firstName = request?.FirstName,
                lastName = request?.LastName,
                email = request?.Email,
                password = request?.Password,
                role = request?.Role,
                status = "pending",
                createdAt = DateTime.UtcNow.ToString("o")
            };

            await _userRegisterService.CreateUserAsync(userDoc);
            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]   //Because you are sending sensitive data (password). GET is not safe for credentials.
        /*
            If you did this:

            GET /api/auth/login?username=john&password=123456


            Problems:

            🔓 Password appears in URL

            📜 URLs get stored in:

            Browser history

            Server logs

            Proxy logs

            Analytics tools

            🔁 URLs can be cached

            🔗 URLs can be accidentally shared

            That is a serious security risk.

            With POST:

                Data is sent in request body

                Not visible in URL

                Not stored in browser history

                Not cached

                Designed for operations that process data
        */
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Username and password are required.");
            
            var user = await _userLoginService.GetUserByUsername(request.Username);
            Console.WriteLine("user: ", user);

            if(user == null)
                return Unauthorized("Invalid credentials");
            
            if (user.Password != request.Password)
            return Unauthorized("Invalid credentials.");

            return Ok(new
            {
                user._Id,
                user.Username,
                user.Role
            });
        }
    }
}