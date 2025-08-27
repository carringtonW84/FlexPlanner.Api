using FlexPlanner.Api.DTOs;

namespace FlexPlanner.Api.Repositories
{
    public interface IConfigurationRepository
    {
        Task<List<OptionDto>> GetTeamsAsync();
        Task<List<TeamCheckboxDto>> GetTeamsWithUserCheckboxesAsync(Guid userId);
        Task<List<OptionDto>> GetVacationTypesAsync();
    }
}
