using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using ChatDashboard.Api.DTOs;
using ChatDashboard.Api.Services;
using ChatDashboard.Api.Models;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DotNetEnv;

namespace ChatDashboard.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserRegisterService _userRegisterService;
        private readonly UserLoginService _userLoginService;

        //just to create public key for once
        private readonly MessageEncryptionService _encryptionService;

        public AuthController(UserRegisterService userRegisterService, UserLoginService userLoginService, MessageEncryptionService encryptionService)
        {
            _userRegisterService = userRegisterService;
            _userLoginService = userLoginService;
            _encryptionService = encryptionService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {   
            if ( string.IsNullOrWhiteSpace(request.Type) ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.RequestedRole))
            {
                return BadRequest("All fields are required.");
            }

            var userDoc = new UserDocument
            {
                _id = Guid.NewGuid().ToString(),
                type = request?.Type,    
                username = request?.Username,
                firstName = request?.FirstName,
                lastName = request?.LastName,
                email = request?.Email,
                password = BCrypt.Net.BCrypt.HashPassword(request?.Password),
                role = (string?)null, //anonymous types in C# are strongly typed at compile time.
                requestedRole = request?.RequestedRole?.ToLower(),
                status = "pending",
                createdAt = DateTime.UtcNow.ToString("o")
            };

            await _userRegisterService.CreateUserAsync(userDoc);
            return Ok(new { message = "Registration request sent to admin for approval" });
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
            
            // Fetch user from DB (CouchDB)
            var user = await _userLoginService.GetUserByUsername(request.Username);

            if (user.Status != "approved")
            {
                return Unauthorized("Your account is waiting for admin approval");
            }

            if(user == null)
                return Unauthorized("Invalid credentials");
            
            bool isValidPassword = false;

            // CASE 1: already hashed password
            if (user.Password.StartsWith("$2"))
            {
                isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            }
            // CASE 2: old plaintext password (legacy users)
            else
            {
                isValidPassword = user.Password == request.Password;

                // If correct → upgrade to hashed automatically
                if (isValidPassword)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    await _userLoginService.UpdateUserPassword(user._Id, user.Password);
                }
            }
            
            if (!isValidPassword)
                return Unauthorized("Invalid credentials.");

            //Need to perform hashed passwords
            // // Verify password (assumes hashed password in DB)
            // if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password)) 
            //     return Unauthorized("Invalid credentials.");

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Environment.GetEnvironmentVariable("JWT_SECRET"); 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user._Id),
                    new Claim("username", user.Username),
                    new Claim("role", user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = new SigningCredentials(
                                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), 
                                        SecurityAlgorithms.HmacSha256Signature
                                    )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                user._Id,
                user.Username,
                user.Role,
                Token = tokenHandler.WriteToken(token)
            });
        }
    }
}