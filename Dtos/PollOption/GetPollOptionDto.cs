namespace WebApiTutorial.Dtos;

public class GetPollOptionDto {
    public string Text { get; set; }
    public List<GetUserDto> VotedUsers { get; set; }
}