using DoAnData.Data;
using DoAnData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DoAnData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DoAnDataContext _context;
        private readonly IConfiguration _configuration;

        public UserController(DoAnDataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // POST: api/User/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new { message = "Tên đăng nhập và mật khẩu là bắt buộc." });
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

            if (user == null || user.Password != loginRequest.Password) // Giả sử mật khẩu không mã hóa
            {
                return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không đúng." });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        // POST: api/User/register-seller
        [HttpPost("register-seller")]
        public async Task<ActionResult<User>> RegisterSeller([FromBody] User seller)
        {
            if (seller == null || string.IsNullOrEmpty(seller.Username) || string.IsNullOrEmpty(seller.Password) || string.IsNullOrEmpty(seller.Email))
            {
                return BadRequest(new { message = "Username, Password và Email là bắt buộc." });
            }

            if (_context.Users.Any(u => u.Username == seller.Username))
            {
                return BadRequest(new { message = "Username đã tồn tại." });
            }
            if (_context.Users.Any(u => u.Email == seller.Email))
            {
                return BadRequest(new { message = "Email đã tồn tại." });
            }

            seller.Role = "seller";
            seller.Status = "pending"; // Chờ admin duyệt
            seller.CreatedAt = DateTime.Now;

            _context.Users.Add(seller);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Login), new { username = seller.Username }, new { message = "Đăng ký thành công, chờ admin duyệt." });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured in appsettings.json");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
