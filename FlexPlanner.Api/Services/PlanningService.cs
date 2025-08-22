using FlexPlanner.Api.DTOs;
using FlexPlanner.Api.Repositories;

namespace FlexPlanner.Api.Services
{
    public class PlanningService : IPlanningService
    {
        private readonly IPlanningRepository _planningRepository;
        private readonly IUserRepository _userRepository;

        public PlanningService(IPlanningRepository planningRepository, IUserRepository userRepository)
        {
            _planningRepository = planningRepository;
            _userRepository = userRepository;
        }

        public async Task<List<PlanningDayDto>> GetMonthlyPlanningAsync(Guid userId, int year, int month)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var planning = new List<PlanningDayDto>();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var dayStatus = await GetDayStatusAsync(userId, date);
                planning.Add(dayStatus);
            }

            return planning;
        }

        public async Task UpdateDayPlanningAsync(Guid userId, UpdatePlanningRequest request)
        {
            await _planningRepository.UpdateUserPlanningAsync(userId, request.Date, request.StatusCode, request.Notes);
        }

        public async Task<WeeklyScheduleDto> GetWeeklyScheduleAsync(Guid userId)
        {
            var schedules = await _planningRepository.GetWeeklyScheduleAsync(userId);

            var schedule = new Dictionary<string, bool>
            {
                ["lundi"] = schedules.FirstOrDefault(s => s.WeekDayId == 1)?.IsOnsite ?? true,
                ["mardi"] = schedules.FirstOrDefault(s => s.WeekDayId == 2)?.IsOnsite ?? true,
                ["mercredi"] = schedules.FirstOrDefault(s => s.WeekDayId == 3)?.IsOnsite ?? false,
                ["jeudi"] = schedules.FirstOrDefault(s => s.WeekDayId == 4)?.IsOnsite ?? true,
                ["vendredi"] = schedules.FirstOrDefault(s => s.WeekDayId == 5)?.IsOnsite ?? false
            };

            return new WeeklyScheduleDto { Schedule = schedule };
        }

        public async Task UpdateWeeklyScheduleAsync(Guid userId, WeeklyScheduleDto schedule)
        {
            var dayMappings = new Dictionary<string, int>
            {
                ["lundi"] = 1,
                ["mardi"] = 2,
                ["mercredi"] = 3,
                ["jeudi"] = 4,
                ["vendredi"] = 5
            };

            foreach (var day in schedule.Schedule)
            {
                if (dayMappings.TryGetValue(day.Key, out var dayId))
                {
                    await _planningRepository.UpdateWeeklyScheduleAsync(userId, dayId, day.Value);
                }
            }
        }

        private async Task<PlanningDayDto> GetDayStatusAsync(Guid userId, DateTime date)
        {
            // This would use the PostgreSQL function get_user_day_status
            var status = await _planningRepository.GetDayStatusAsync(userId, date);

            var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
            var isHoliday = await _planningRepository.IsHolidayAsync(date);

            return new PlanningDayDto
            {
                Date = date,
                StatusCode = status.Code,
                StatusName = status.Name,
                StatusEmoji = status.Emoji,
                StatusColor = status.ColorClass,
                Notes = status.Notes,
                CanModify = !isWeekend && !isHoliday
            };
        }
    }
}
