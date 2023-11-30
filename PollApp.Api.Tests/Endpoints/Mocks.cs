using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PollApp.Api;
using PollApp.Api.Dtos;
using PollApp.Api.Models;
using PollApp.Api.Services;

class UserServiceMock : IUserService
{
    public List<User> Users { get; set; } = new();

    private IMapper _mapper;
    private IConfiguration _configuration;

    public UserServiceMock(IMapper mapper) {
        _mapper = mapper;

        
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        for (int i = 0; i < 3; i++) {
            var user = new User() {
                Username = "User" + (i+1),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass" + (i+1))
            };

            Users.Add(user);
        }
    }

    public async Task<GetUserDto> ByUsername(string username)
    {
        var user = Users.FirstOrDefault(u => u.Username == username) ?? throw new Exception("");
        return _mapper.Map<GetUserDto>(user);
    }

    public async Task<IEnumerable<GetUserDto>> GetAll()
    {
        return Users.Select(user => _mapper.Map<GetUserDto>(user));
    }

    public async Task<string> Login(UserDto user)
    {
        var target = Users.FirstOrDefault(u => u.Username == user.Username) ?? throw new Exception("");
        if (!BCrypt.Net.BCrypt.Verify(user.Password, target.PasswordHash)) throw new Exception("");

        // TODO return actual token?
        return CreateToken(target);
    }

    public async Task<GetUserDto> Register(UserDto user)
    {
        var taken = Users.FirstOrDefault(u => u.Username == user.Username);
        if (taken != null) throw new Exception();

        string passHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
        var newUser = new User {
            Username = user.Username,
            PasswordHash = passHash
        };

        Users.Add(newUser);
        return _mapper.Map<GetUserDto>(Users[0]);
    }

    private string CreateToken(User user) {
        var claims = new List<Claim>(){
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

class PollServiceMock : IPollService
{
    private IMapper _mapper;
    public List<Poll> Polls { get; set; } = new();

    private IUserService _userService;

    public PollServiceMock(IMapper mapper, IUserService userService) {
        // var mC = new MapperConfiguration(cfg => {
        //     cfg.AddProfile(new AutoMapperProfile());
        // });
        // _mapper = new Mapper(mC);
        _mapper = mapper;

        _userService = userService;

        for (int i = 0; i < 10; i++) {
            var poll = new Poll() {
                ID = i+1,
                Title = "Poll" + (i+1),
                Text = "Poll text" + (i+1),
                Options = new() {
                    new() {
                        Text = "Option1",
                    },
                    new() {
                        Text = "Option2",
                    }
                }
            };

            Polls.Add(poll);
        }
    }
    public async Task<IEnumerable<GetPollDto>> Add(AddPollDto poll)
    {
        if (poll.Options.Count == 0) throw new Exception();
        Polls.Add(_mapper.Map<Poll>(poll));
        return await GetAll();
    }

    public async Task<GetPollDto> ByID(int id)
    {
        var result = Polls.SingleOrDefault(poll => poll.ID == id) ?? throw new Exception();
        return _mapper.Map<GetPollDto>(result);
    }

    public async Task<IEnumerable<GetPollDto>> GetAll()
    {
        return Polls.Select(poll => _mapper.Map<GetPollDto>(poll));
    }

    public async Task<GetPollDto> VoteFor(string username, int pollID, string option)
    {
        var poll = Polls.SingleOrDefault(poll => poll.ID == pollID) ?? throw new Exception();
        foreach (var o in poll.Options) {
            var user = o.VotedUsers.FirstOrDefault(u => u.Username == username);
            if (user != null) throw new Exception();
        }
        var op = poll.Options.FirstOrDefault(po => po.Text == option) ?? throw new Exception("");
        var voter = await _userService.ByUsername(username);
        // TODO bad!
        op.VotedUsers.Add(new User {
            Username = voter.Username
        });
        return _mapper.Map<GetPollDto>(poll);
    }
}