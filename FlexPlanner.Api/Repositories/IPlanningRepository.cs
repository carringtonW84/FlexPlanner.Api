using FlexPlanner.Api.Models;

namespace FlexPlanner.Api.Repositories
{
    public interface IPlanningRepository
    {
        Task<(string Code, string Name, string? Emoji, string? ColorClass, string? Notes)> GetDayStatusAsync(Guid userId, DateTime date);
        Task UpdateUserPlanningAsync(Guid userId, DateTime date, string statusCode, string? notes);
        Task<List<UserWeeklySchedule>> GetWeeklyScheduleAsync(Guid userId);
        Task UpdateWeeklyScheduleAsync(Guid userId, int dayId, bool isOnsite);
        Task<bool> IsHolidayAsync(DateTime date);
    }
}
