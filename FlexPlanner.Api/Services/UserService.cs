using FlexPlanner.Api.DTOs;
using FlexPlanner.Api.Models;
using FlexPlanner.Api.Repositories;

namespace FlexPlanner.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Velocity = user.Velocity,
                Team = user.Team != null ? new TeamDto
                {
                    Id = user.Team.Id,
                    Code = user.Team.Code,
                    Name = user.Team.Name,
                    Description = user.Team.Description
                } : null
            };
        }

        public async Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequest request)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            if (!string.IsNullOrEmpty(request.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            if (request.TeamId.HasValue)
            {
                user.TeamId = request.TeamId.Value;
            }

            if (request.Velocity.HasValue)
            {
                user.Velocity = request.Velocity.Value;
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user);

            return await GetUserByIdAsync(userId);
        }

        public async Task<List<TeamMemberPresenceDto>> GetTeamMembersAsync(Guid teamId)
        {
            var users = await _userRepository.GetUsersByTeamIdAsync(teamId);
            var today = DateTime.Today;

            var members = new List<TeamMemberPresenceDto>();

            foreach (var user in users)
            {
                var dayStatus = await GetUserDayStatus(user, today);

                members.Add(new TeamMemberPresenceDto
                {
                    Id = user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                    Status = dayStatus.IsPresent ? "present" : "absent",
                    Location = dayStatus.Location,
                    Team = user.Team?.Name ?? "No Team"
                });
            }

            return members;
        }

        private async Task<(bool IsPresent, string Location)> GetUserDayStatus(User user, DateTime date)
        {
            // Check specific planning first
            var planning = user.PlanningEntries.FirstOrDefault(p => p.Date.Date == date.Date);
            if (planning != null)
            {
                var isPresent = planning.Status.Code != "VACATION";
                var location = planning.Status.Code switch
                {
                    "ONSITE" => "sur site",
                    "REMOTE" => "télétravail",
                    "VACATION" => "congés",
                    _ => "inconnu"
                };
                return (isPresent, location);
            }

            // Check vacations
            var vacation = user.Vacations.FirstOrDefault(v =>
                date.Date >= v.StartDate.Date &&
                date.Date <= v.EndDate.Date);

            if (vacation != null)
            {
                return (false, "congés");
            }

            // Check if it's weekend
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                return (false, "week-end");
            }

            // Use weekly schedule
            var dayOfWeek = (int)date.DayOfWeek;
            var weeklySchedule = user.WeeklySchedules.FirstOrDefault(w => w.WeekDayId == dayOfWeek);

            if (weeklySchedule != null)
            {
                return (true, weeklySchedule.IsOnsite ? "sur site" : "télétravail");
            }

            // Default: onsite
            return (true, "sur site");
        }
    }
}
