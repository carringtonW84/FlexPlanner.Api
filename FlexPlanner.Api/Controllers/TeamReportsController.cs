using FlexPlanner.Api.DTOs;
using FlexPlanner.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlexPlanner.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamReportsController : ControllerBase
    {
        private readonly ITeamReportService _teamReportService;

        public TeamReportsController(ITeamReportService teamReportService)
        {
            _teamReportService = teamReportService;
        }

        [HttpGet("{teamId}/planning")]
        public async Task<ActionResult<TeamPlanningReportDto>> GetTeamPlanning(
            Guid teamId,
            DateTime startDate,
            DateTime endDate)
        {
            // Vérifier que la plage ne dépasse pas 31 jours
            if ((endDate - startDate).Days > 31)
            {
                return BadRequest("La plage de dates ne peut pas dépasser 31 jours");
            }

            var report = await _teamReportService.GetTeamPlanningReportAsync(teamId, startDate, endDate);
            return Ok(report);
        }

        [HttpGet("{teamId}/planning/export")]
        public async Task<IActionResult> ExportTeamPlanningToExcel(
            Guid teamId,
            DateTime startDate,
            DateTime endDate)
        {
            // Vérifier que la plage ne dépasse pas 31 jours
            if ((endDate - startDate).Days > 31)
            {
                return BadRequest("La plage de dates ne peut pas dépasser 31 jours");
            }

            var excelData = await _teamReportService.ExportTeamPlanningToExcelAsync(teamId, startDate, endDate);

            var teamName = await _teamReportService.GetTeamNameAsync(teamId);
            var fileName = $"Planning_{teamName}_{startDate:yyyyMMdd}-{endDate:yyyyMMdd}.xlsx";

            return File(
                excelData,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
        }
    }
}