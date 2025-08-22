using FlexPlanner.Api.DTOs;
using FlexPlanner.Api.Models;
using FlexPlanner.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlexPlanner.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlanningController : ControllerBase
    {
        private readonly IPlanningService _planningService;

        public PlanningController(IPlanningService planningService)
        {
            _planningService = planningService;
        }

        [HttpGet("monthly")]
        public async Task<ActionResult<List<PlanningDayDto>>> GetMonthlyPlanning(int year, int month)
        {
            var userId = GetCurrentUserId();
            var planning = await _planningService.GetMonthlyPlanningAsync(userId, year, month);
            return Ok(planning);
        }

        [HttpPut("day")]
        public async Task<ActionResult> UpdateDayPlanning(UpdatePlanningRequest request)
        {
            var userId = GetCurrentUserId();
            await _planningService.UpdateDayPlanningAsync(userId, request);
            return NoContent();
        }

        [HttpGet("weekly-schedule")]
        public async Task<ActionResult<WeeklyScheduleDto>> GetWeeklySchedule()
        {
            var userId = GetCurrentUserId();
            var schedule = await _planningService.GetWeeklyScheduleAsync(userId);
            return Ok(schedule);
        }

        [HttpPut("weekly-schedule")]
        public async Task<ActionResult> UpdateWeeklySchedule(WeeklyScheduleDto schedule)
        {
            var userId = GetCurrentUserId();
            await _planningService.UpdateWeeklyScheduleAsync(userId, schedule);
            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
        }
    }
}
