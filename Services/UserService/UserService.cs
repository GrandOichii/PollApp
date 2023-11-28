using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebApiTutorial.Services;

public class UserService : IUserService
{
    private IConfiguration _configuration;
    private IMapper _mapper;
    private DataContext _ctx;

    public UserService(IConfiguration configuration, IMapper mapper, DataContext ctx) {
        _configuration = configuration;
        _mapper = mapper;
        _ctx = ctx;
    }


    public async Task<GetUserDto> ByUsername(string username)
    {
        var user = await _ctx.Users.FirstAsync(user => user.Username == username);
        return _mapper.Map<GetUserDto>(user);
    }

    public async Task<IEnumerable<GetUserDto>> GetAll()
    {
        var list = await _ctx.Users.ToListAsync();
        return list.Select(user => _mapper.Map<GetUserDto>(user));
    }

    public async Task<string> Login(UserDto user)
    {
        var users = await _ctx.Users.ToListAsync();
        foreach (var u in users) {
            if (u.Username != user.Username) continue;

            // i think it is better to continue, response time could indicate that a user with the specified username exists
            if (!BCrypt.Net.BCrypt.Verify(user.Password, u.PasswordHash)) continue;

            string token = CreateToken(u);

            return token;
        }
        throw new Exception("Failed to authenticate");
    }

    public async Task<GetUserDto> Register(UserDto user)
    {
        string passHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
        var newUser = new User() {
            Username = user.Username,
            PasswordHash = passHash    
        };

        await _ctx.Users.AddAsync(newUser);
        await _ctx.SaveChangesAsync();
        return await ByUsername(user.Username);
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