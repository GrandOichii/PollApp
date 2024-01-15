using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace PollApp.Api.Controllers {
    public class Vote {
        public int PollID { get; set; }
        public string Option { get; set; } = string.Empty;
    }

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
            return Ok( await _pollService.GetAll() );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ByID(int id) {
            try {
                var poll = await _pollService.ByID(id);
                return Ok(poll);
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> AddPoll([FromBody] AddPollDto poll) {
            try {
                var polls = await _pollService.Add(poll);
                return Ok( polls );
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("vote"), Authorize]
        public async Task<IActionResult> VoteFor([FromBody] Vote vote) {
            try {
                var userID = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
                return Ok(await _pollService.VoteFor(userID, vote.PollID, vote.Option));
            } catch (Exception e) {
                return BadRequest(e.Message);   
            }
        }
    }

}
