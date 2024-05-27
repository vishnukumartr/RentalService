using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentalService.Dtos;
using RentalService.Models;
using RentalService.Services.InterestService;

namespace RentalService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShowInterestController : ControllerBase
    {
        private readonly IShowInterestService _showInterestSrvc;

        public ShowInterestController(IShowInterestService showInterestService) 
        {
            _showInterestSrvc = showInterestService;
        }

        [HttpPost("ShowInterest")]
        public async Task<ActionResult<string>> PostInterest(ShowInterestDto showInterestDto)
        {
            try
            {
                var interest = await _showInterestSrvc.ShowInterestAsync(showInterestDto);
                return Ok(interest);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("NotInterested/{id}")]
        public async Task<IActionResult> RemoveInterest(int id)
        {
            var success = await _showInterestSrvc.RemoveInterestAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

    }

    
}
