using FlexPlanner.Api.DTOs;

namespace FlexPlanner.Api.Services
{
    public interface IPlanningService
    {
        Task<List<PlanningDayDto>> GetMonthlyPlanningAsync(Guid userId, int year, int month);
        Task UpdateDayPlanningAsync(Guid userId, UpdatePlanningRequest request);
        Task<WeeklyScheduleDto> GetWeeklyScheduleAsync(Guid userId);
        Task UpdateWeeklyScheduleAsync(Guid userId, WeeklyScheduleDto schedule);
    }
}
