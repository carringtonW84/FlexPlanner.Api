using FlexPlanner.Api.Data;
using FlexPlanner.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FlexPlanner.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FlexPlannerDbContext _context;

        public UserRepository(FlexPlannerDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Team)
                .Include(u => u.WeeklySchedules)
                .Include(u => u.PlanningEntries)
                    .ThenInclude(p => p.Status)
                .Include(u => u.Vacations)
                    .ThenInclude(v => v.VacationType)
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
        }

        public async Task<List<User>> GetUsersByTeamIdAsync(Guid teamId)
        {
            return await _context.Users
                .Include(u => u.Team)
                .Include(u => u.WeeklySchedules)
                .Include(u => u.PlanningEntries)
                    .ThenInclude(p => p.Status)
                .Include(u => u.Vacations)
                .Where(u => u.TeamId == teamId && u.IsActive)
                .ToListAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
