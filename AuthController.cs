
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RaceDayApi1.Data;
using RaceDayApi1.Models;

namespace RaceDayApi1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<Users> _passwordHasher;

        public AuthController(
            AppDbContext db,
            IConfiguration configuration,
            IPasswordHasher<Users> passwordHasher)
        {
            _db = db;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public class RegisterRequest
        {
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

       
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(
                    "Full name, email and password are required.");
            }

            if (request.Password.Length < 8)
            {
                return BadRequest(
                    "Password must be at least 8 characters long.");
            }

            string email = request.Email.Trim();

            bool emailExists = await _db.Users
                .AnyAsync(u => u.Email == email);

            if (emailExists)
            {
                return Conflict(
                    "An account with this email already exists.");
            }

            var user = new Users
            {
                FullName = request.FullName.Trim(),
                Email = email,
                Role = "Participant"
            };

            
            user.Password = _passwordHasher.HashPassword(
                user, request.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Registration successful.",
                userID = user.UserID,
                fullName = user.FullName,
                email = user.Email,
                role = user.Role
            });
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(
                    "Email and password are required.");
            }

            string email = request.Email.Trim();

            var user = await _db.Users
                .SingleOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return Unauthorized(
                    "Invalid email or password.");
            }

            PasswordVerificationResult passwordResult;

            try
            {
                passwordResult =
                    _passwordHasher.VerifyHashedPassword(
                        user,
                        user.Password,
                        request.Password);
            }
            catch (FormatException)
            {
                return Unauthorized(
                    "Invalid email or password.");
            }
            catch (ArgumentException)
            {
                return Unauthorized(
                    "Invalid email or password.");
            }

            if (passwordResult ==
                PasswordVerificationResult.Failed)
            {
                return Unauthorized(
                    "Invalid email or password.");
            }

            string? jwtKey = _configuration["Jwt:Key"];
            string? issuer = _configuration["Jwt:Issuer"];
            string? audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey) ||
                Encoding.UTF8.GetByteCount(jwtKey) < 32 ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                return StatusCode(
                    500,
                    "JWT configuration is missing or invalid.");
            }

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserID.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return Ok(new
            {
                message = "Login successful.",
                token = new JwtSecurityTokenHandler()
                    .WriteToken(token),
                userID = user.UserID,
                email = user.Email,
                role = user.Role
            });
        }
    }
}