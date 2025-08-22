using FlexPlanner.Api.DTOs;
using FlexPlanner.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlexPlanner.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VacationsController : ControllerBase
    {
        private readonly IVacationService _vacationService;

        public VacationsController(IVacationService vacationService)
        {
            _vacationService = vacationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<VacationDto>>> GetVacations()
        {
            var userId = GetCurrentUserId();
            var vacations = await _vacationService.GetUserVacationsAsync(userId);
            return Ok(vacations);
        }

        [HttpPost]
        public async Task<ActionResult<VacationDto>> CreateVacation(CreateVacationRequest request)
        {
            var userId = GetCurrentUserId();
            var vacation = await _vacationService.CreateVacationAsync(userId, request);
            return CreatedAtAction(nameof(GetVacations), new { id = vacation.Id }, vacation);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVacation(Guid id)
        {
            var userId = GetCurrentUserId();
            await _vacationService.DeleteVacationAsync(userId, id);
            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
        }
    }
}
