using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace PollApp.Api.Services;

public class PollService : IPollService
{
    private IMapper _mapper;
    private DataContext _ctx;

    public PollService(IMapper mapper, DataContext ctx) {
        _mapper = mapper;
        _ctx = ctx;
    }

    public async Task<IEnumerable<GetPollDto>> Add([FromBody] AddPollDto poll)
    {
        if (poll.Options.Count == 0) throw new Exception("Can't create poll with no options");
        
        await _ctx.AddAsync(_mapper.Map<Poll>(poll));
        await _ctx.SaveChangesAsync();
        return await GetAll();
    }

    public async Task<GetPollDto> ByID(int id)
    {
        var poll = await _ctx.Polls
                                .Include(poll => poll.Options)
                                    .ThenInclude(pollOption => pollOption.VotedUsers)
                                .FirstOrDefaultAsync(poll => poll.ID == id);
        if (poll is null) throw new Exception("No poll with id: " + id);
        return _mapper.Map<GetPollDto>(poll);
    }

    public async Task<IEnumerable<GetPollDto>> GetAll()
    {
        var list = await _ctx.Polls
            .Include(poll => poll.Options)
                .ThenInclude(pollOption => pollOption.VotedUsers)
            .ToListAsync();
        return list
            .Select(poll => _mapper.Map<GetPollDto>(poll));
    }

    public async Task<GetPollDto> VoteFor(string username, int pollID, string option)
    {
        // TODO fix incorrect exceptions
        var poll = await _ctx.Polls
                            .Include(poll => poll.Options)
                                .ThenInclude(pollOption => pollOption.VotedUsers)
                            .FirstAsync(poll => poll.ID == pollID); // here
        foreach (var o in poll.Options) {
            var me = o.VotedUsers.Find(user => user.Username == username);
            if (me != null) {
                throw new Exception(username + " has already voted in poll " + pollID);
            }
        }
        foreach (var o in poll.Options) {
            if (o.Text != option) continue;
            var me = o.VotedUsers.Find(user => user.Username == username);

            var newVoter = await _ctx.Users.FirstAsync(user => user.Username == username);
            o.VotedUsers.Add(newVoter);
            _ctx.SaveChanges();
            return await ByID(pollID);
        }
        // and here
        throw new Exception("No option " + option + " in poll " + pollID);
    }
}