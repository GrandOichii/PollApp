using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace PollApp.Api.Controllers {

    [ApiController]
    [Route("api/Users")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService) {
            _userService = userService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            return Ok(await _userService.GetAll());
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto user) {
            try {
                var u = await _userService.Register(user); 
                return Ok(u); 
            } catch (Exception e) {
                return Conflict(e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto user) {
            try {
                var result = await _userService.Login(user);
                return Ok(result);
            } catch (Exception e) {
                return Unauthorized(e.Message);
            }
        }
    }

}
