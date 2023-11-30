namespace PollApp.Api.Dtos;

public class GetPollOptionDto {
    public string Text { get; set; } = string.Empty;
    public List<GetUserDto> VotedUsers { get; set; } = new();
}