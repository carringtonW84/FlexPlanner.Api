using FlexPlanner.Api.DTOs;

namespace FlexPlanner.Api.Services
{
    public interface IVacationService
    {
        Task<List<VacationDto>> GetUserVacationsAsync(Guid userId);
        Task<VacationDto> CreateVacationAsync(Guid userId, CreateVacationRequest request);
        Task DeleteVacationAsync(Guid userId, Guid vacationId);
    }
}
