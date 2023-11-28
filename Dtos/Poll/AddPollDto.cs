namespace WebApiTutorial.Dtos;

public class AddPollDto {
    public string Title { get; set; }
    public string Text { get; set; }

    public List<AddPollOptionDto> Options { get; set; }
}