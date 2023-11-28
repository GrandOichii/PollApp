
namespace WebApiTutorial.Services;

public interface IPollService {
    public Task<IEnumerable<GetPollDto>> GetAll();
    public Task<GetPollDto> ByID(int id);
    public Task<IEnumerable<GetPollDto>> Add(AddPollDto poll);
    public Task<GetPollDto> Update(UpdatePollDto updatedPoll);

    public Task<GetPollDto> VoteFor(string username, int pollID, string option);
}