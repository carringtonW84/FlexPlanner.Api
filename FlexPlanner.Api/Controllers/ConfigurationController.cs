using FlexPlanner.Api.DTOs;
using FlexPlanner.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FlexPlanner.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationRepository _configRepository;

        public ConfigurationController(IConfigurationRepository configRepository)
        {
            _configRepository = configRepository;
        }

        [HttpGet("teams")]
        public async Task<ActionResult<List<OptionDto>>> GetTeams()
        {
            var teams = await _configRepository.GetTeamsAsync();
            return Ok(teams);
        }

        [HttpGet("teams/checkboxes")]
        [Authorize]
        public async Task<ActionResult<List<TeamCheckboxDto>>> GetTeamsWithCheckboxes()
        {
            var userId = GetCurrentUserId();
            var teams = await _configRepository.GetTeamsWithUserCheckboxesAsync(userId);
            return Ok(teams);
        }

        [HttpGet("vacation-types")]
        public async Task<ActionResult<List<OptionDto>>> GetVacationTypes()
        {
            var types = await _configRepository.GetVacationTypesAsync();
            return Ok(types);
        }

        [HttpGet("months")]
        public ActionResult<List<OptionDto>> GetMonths()
        {
            var months = new List<OptionDto>();
            for (int i = 0; i < 12; i++)
            {
                months.Add(new OptionDto
                {
                    Value = i.ToString(),
                    Label = new DateTime(2000, i + 1, 1).ToString("MMMM", new CultureInfo("fr-FR"))
                });
            }
            return Ok(months);
        }

        [HttpGet("years")]
        public ActionResult<List<OptionDto>> GetYears()
        {
            var years = new List<OptionDto>();
            for (int i = 2020; i <= 2030; i++)
            {
                years.Add(new OptionDto
                {
                    Value = i.ToString(),
                    Label = i.ToString()
                });
            }
            return Ok(years);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
        }
    }
}