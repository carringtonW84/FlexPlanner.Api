using FlexPlanner.Api.DTOs;
using FlexPlanner.Api.Models;

namespace FlexPlanner.Api.Repositories
{
    public interface IVacationRepository
    {
        Task<List<UserVacation>> GetUserVacationsAsync(Guid userId);
        Task<UserVacation> CreateVacationAsync(Guid userId, CreateVacationRequest request);
        Task DeleteVacationAsync(Guid userId, Guid vacationId);
    }
}
