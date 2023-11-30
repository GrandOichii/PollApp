using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            try {
                var poll = await _pollService.ByID(id);
                return Ok(poll);
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> AddPoll([FromBody] AddPollDto poll) {
            return Ok( await _pollService.Add(poll));
        }

        [HttpPut, Authorize]
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
