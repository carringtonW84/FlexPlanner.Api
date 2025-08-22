using FlexPlanner.Api.DTOs;
using FlexPlanner.Api.Repositories;

namespace FlexPlanner.Api.Services
{
    public class VacationService : IVacationService
    {
        private readonly IVacationRepository _vacationRepository;

        public VacationService(IVacationRepository vacationRepository)
        {
            _vacationRepository = vacationRepository;
        }

        public async Task<List<VacationDto>> GetUserVacationsAsync(Guid userId)
        {
            var vacations = await _vacationRepository.GetUserVacationsAsync(userId);

            return vacations.Select(v => new VacationDto
            {
                Id = v.Id,
                Type = v.VacationType.Name,
                StartDate = v.StartDate,
                EndDate = v.EndDate,
                Status = v.Status,
                Notes = v.Notes
            }).ToList();
        }

        public async Task<VacationDto> CreateVacationAsync(Guid userId, CreateVacationRequest request)
        {
            var vacation = await _vacationRepository.CreateVacationAsync(userId, request);

            return new VacationDto
            {
                Id = vacation.Id,
                Type = vacation.VacationType.Name,
                StartDate = vacation.StartDate,
                EndDate = vacation.EndDate,
                Status = vacation.Status,
                Notes = vacation.Notes
            };
        }

        public async Task DeleteVacationAsync(Guid userId, Guid vacationId)
        {
            await _vacationRepository.DeleteVacationAsync(userId, vacationId);
        }
    }
}
