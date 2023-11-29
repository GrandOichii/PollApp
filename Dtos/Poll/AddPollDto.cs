namespace WebApiTutorial.Dtos;

public class AddPollDto {
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;

    public List<AddPollOptionDto> Options { get; set; } = new();
}