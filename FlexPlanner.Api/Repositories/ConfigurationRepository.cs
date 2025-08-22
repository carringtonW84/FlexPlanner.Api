using FlexPlanner.Api.Data;
using FlexPlanner.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FlexPlanner.Api.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly FlexPlannerDbContext _context;

        public ConfigurationRepository(FlexPlannerDbContext context)
        {
            _context = context;
        }

        public async Task<List<OptionDto>> GetTeamsAsync()
        {
            return await _context.Teams
                .Where(t => t.IsActive)
                .Select(t => new OptionDto
                {
                    Value = t.Id.ToString(),
                    Label = t.Name
                })
                .ToListAsync();
        }

        public async Task<List<OptionDto>> GetVacationTypesAsync()
        {
            return await _context.VacationTypes
                .Where(vt => vt.IsActive)
                .Select(vt => new OptionDto
                {
                    Value = vt.Code,
                    Label = $"{vt.Emoji} {vt.Name}"
                })
                .ToListAsync();
        }
    }
}
