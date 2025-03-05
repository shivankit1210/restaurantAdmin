using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace RestaurantAdmin.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // ✅ Allow anonymous access to login
        [HttpPost("login")]
        [AllowAnonymous]  // <-- Add this to allow login without authentication
        public IActionResult Login([FromBody] AdminLoginModel admin)
        {
            if (admin == null || string.IsNullOrEmpty(admin.Username) || string.IsNullOrEmpty(admin.Password))
            {
                return BadRequest(new { message = "Invalid request data" });
            }

            // ✅ Replace this with real database authentication logic
            if (admin.Username == "admin" && admin.Password == "admin123") 
            {
                var token = GenerateJwtToken(admin.Username);
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Invalid credentials" });
        }

        private string GenerateJwtToken(string username)
        {
            string? jwtKey = _configuration.GetValue<string>("Jwt:Key");
            string? issuer = _configuration.GetValue<string>("Jwt:Issuer");
            string? audience = _configuration.GetValue<string>("Jwt:Audience");

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT configuration is missing or invalid.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class AdminLoginModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}