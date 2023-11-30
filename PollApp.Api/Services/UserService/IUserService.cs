
namespace PollApp.Api.Services;

public interface IUserService {
    public Task<IEnumerable<GetUserDto>> GetAll();
    public Task<GetUserDto> ByUsername(string username);
    public Task<GetUserDto> Register(UserDto user);
    public Task<string> Login(UserDto user);
}