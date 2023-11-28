namespace WebApiTutorial.Dtos;

public class GetPollDto {
    public int ID { get; set; }

    public string Title { get; set; }
    public string Text { get; set; }

    public List<PollOption> Options { get; set; }
}