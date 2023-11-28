using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace WebApiTutorial.Controllers {

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly static User _admin = new();

        private IConfiguration _configuration;

        public AuthController(IConfiguration configuration) {
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto user) {
            string passHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _admin.Username = user.Username;
            _admin.PasswordHash = passHash;

            return Ok(_admin);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto user) {
            if (_admin.Username != user.Username) return BadRequest("User not found");

            if (!BCrypt.Net.BCrypt.Verify(user.Password, _admin.PasswordHash)) return BadRequest("Incorrect password");

            string token = CreateToken(_admin);

            return Ok(token);
        }

        [HttpGet("test"), Authorize]
        public async Task<IActionResult> Test() {
            return Ok(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value);
        }

        private string CreateToken(User user) {
            List<Claim> claims = new List<Claim>{
                new(ClaimTypes.Name, user.Username),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }

}
