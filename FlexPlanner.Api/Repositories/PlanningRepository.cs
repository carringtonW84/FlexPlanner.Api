using FlexPlanner.Api.Data;
using FlexPlanner.Api.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace FlexPlanner.Api.Repositories
{
    public class PlanningRepository : IPlanningRepository
    {
        private readonly FlexPlannerDbContext _context;

        public PlanningRepository(FlexPlannerDbContext context)
        {
            _context = context;
        }

        public async Task<(string Code, string Name, string? Emoji, string? ColorClass, string? Notes)> GetDayStatusAsync(Guid userId, DateTime date)
        {
            // Use the PostgreSQL function get_user_day_status
            var sql = "SELECT * FROM flexplanner.get_user_day_status(@userId, @date)";
            var parameters = new[]
            {
                new NpgsqlParameter("@userId", userId),
                new NpgsqlParameter("@date", NpgsqlTypes.NpgsqlDbType.Date) { Value = date.Date }
            };

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddRange(parameters);

            await _context.Database.OpenConnectionAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return (
                    reader.GetString("status_code"),
                    reader.GetString("status_name"),
                    reader.IsDBNull("status_emoji") ? null : reader.GetString("status_emoji"),
                    reader.IsDBNull("status_color") ? null : reader.GetString("status_color"),
                    null // Notes would come from user_planning table if needed
                );
            }

            return ("ONSITE", "Sur site", "🏢", "bg-green-100 text-green-800 border-green-200", null);
        }

        public async Task UpdateUserPlanningAsync(Guid userId, DateTime date, string statusCode, string? notes)
        {
            var status = await _context.PresenceStatuses
                .FirstOrDefaultAsync(s => s.Code == statusCode);

            if (status == null)
                throw new ArgumentException("Invalid status code");

            var existing = await _context.UserPlanningEntries
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Date.Date == date.Date);

            if (existing != null)
            {
                existing.StatusId = status.Id;
                existing.Notes = notes;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.UserPlanningEntries.Add(new UserPlanning
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Date = date.Date,
                    StatusId = status.Id,
                    Notes = notes
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<UserWeeklySchedule>> GetWeeklyScheduleAsync(Guid userId)
        {
            return await _context.UserWeeklySchedules
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateWeeklyScheduleAsync(Guid userId, int dayId, bool isOnsite)
        {
            var existing = await _context.UserWeeklySchedules
                .FirstOrDefaultAsync(s => s.UserId == userId && s.WeekDayId == dayId);

            if (existing != null)
            {
                existing.IsOnsite = isOnsite;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.UserWeeklySchedules.Add(new UserWeeklySchedule
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    WeekDayId = dayId,
                    IsOnsite = isOnsite
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsHolidayAsync(DateTime date)
        {
            var key = date.ToString("MM-dd");
            return await _context.FrenchHolidays
                .AnyAsync(h => h.HolidayKey == key && h.IsActive);
        }
    }
}
