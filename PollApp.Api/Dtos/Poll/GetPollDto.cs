namespace PollApp.Api.Dtos;

public class GetPollDto {
    public int ID { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;

    public List<GetPollOptionDto> Options { get; set; } = new();
}