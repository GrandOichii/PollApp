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
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService) {
            _userService = userService;
        }
        
        [HttpGet("all")]
        public async Task<IActionResult> GetAll() {
            return Ok(await _userService.GetAll());
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto user) {
            return Ok(await _userService.Register(user)); 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto user) {
            return Ok(await _userService.Login(user));
        }

        [HttpGet("test"), Authorize]
        public async Task<IActionResult> Test() {
            return Ok(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value);
        }
    }

}
