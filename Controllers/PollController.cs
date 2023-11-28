using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class Global {
    public readonly static Global Instance = new();
    private Global() {}

    public List<Poll> Polls { get; set; } = new List<Poll>() {
        new() { Title="Poll1", Text="This is the first poll.", Options=new(){
            new() { Text = "Option1" }, 
            new() { Text = "Option2" }, 
        } },
        new() { Title="Poll2", Text="This is the second poll.", Options=new(){
            new() { Text = "Photos printed",}, 
            new() { Text = "Bogos binted" }, 
        } },
    };
}
namespace WebApiTutorial.Controllers {

    // [ApiController, Authorize]
    [ApiController]
    [Route("api/Polls")]
    public class PollController : ControllerBase
    {

        private IPollService _pollService;
        public PollController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [HttpGet]
        public async Task<IActionResult> All() {            
            return Ok( await _pollService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ByID(int id) {
            return Ok( await 
                _pollService.ByID(id)
            );
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> AddPoll([FromBody] AddPollDto poll) {
            return Ok( await _pollService.Add(poll));
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePoll([FromBody] UpdatePollDto updatedPoll) {
            return Ok(await _pollService.Update(updatedPoll));
        }

        [HttpPut("vote/{id}"), Authorize]
        public async Task<IActionResult> VoteFor(int id, [FromBody] string option) {
            var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
            return Ok(await _pollService.VoteFor(userID, id, option));
        }
    }

}
