using FlexPlanner.Api.DTOs;
using FlexPlanner.Api.Models;
using FlexPlanner.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using FlexPlanner.Api.Data;

namespace FlexPlanner.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly FlexPlannerDbContext _context;

        public UserService(IUserRepository userRepository, FlexPlannerDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            // Récupérer les équipes de l'utilisateur via la relation N-N
            var userTeams = await _context.UserTeams
                .Where(ut => ut.UserId == userId)
                .Include(ut => ut.Team)
                .Select(ut => new TeamDto
                {
                    Id = ut.Team.Id,
                    Code = ut.Team.Code,
                    Name = ut.Team.Name,
                    Description = ut.Team.Description
                })
                .ToListAsync();

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
                } : userTeams.FirstOrDefault(), // Fallback sur la première équipe
                Teams = userTeams
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

            // Mise à jour de l'équipe principale (compatibilité)
            if (request.TeamId.HasValue)
            {
                user.TeamId = request.TeamId.Value;
            }

            // Mise à jour des équipes multiples
            if (request.TeamIds != null)
            {
                // Supprimer toutes les associations existantes
                var existingUserTeams = await _context.UserTeams
                    .Where(ut => ut.UserId == userId)
                    .ToListAsync();

                _context.UserTeams.RemoveRange(existingUserTeams);

                // Ajouter les nouvelles associations
                var newUserTeams = request.TeamIds.Select(teamId => new UserTeam
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TeamId = teamId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();

                await _context.UserTeams.AddRangeAsync(newUserTeams);

                // Mettre à jour l'équipe principale avec la première équipe sélectionnée
                if (request.TeamIds.Any())
                {
                    user.TeamId = request.TeamIds.First();
                }
                else
                {
                    user.TeamId = null;
                }
            }

            if (request.Velocity.HasValue)
            {
                user.Velocity = request.Velocity.Value;
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user);
            await _context.SaveChangesAsync();

            return await GetUserByIdAsync(userId);
        }

        public async Task<List<TeamMemberPresenceDto>> GetTeamMembersAsync(Guid teamId)
        {
            // Mise à jour pour prendre en compte les relations N-N
            var users = await _context.UserTeams
                .Where(ut => ut.TeamId == teamId)
                .Include(ut => ut.User)
                    .ThenInclude(u => u.PlanningEntries)
                        .ThenInclude(p => p.Status)
                .Include(ut => ut.User)
                    .ThenInclude(u => u.Vacations)
                .Include(ut => ut.User)
                    .ThenInclude(u => u.WeeklySchedules)
                .Include(ut => ut.Team)
                .Select(ut => ut.User)
                .Where(u => u.IsActive)
                .ToListAsync();

            var today = DateTime.Today;
            var members = new List<TeamMemberPresenceDto>();

            foreach (var user in users)
            {
                var dayStatus = await GetUserDayStatus(user, today);
                var team = await _context.Teams.FindAsync(teamId);

                members.Add(new TeamMemberPresenceDto
                {
                    Id = user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                    Status = dayStatus.IsPresent ? "present" : "absent",
                    Location = dayStatus.Location,
                    Team = team?.Name ?? "No Team"
                });
            }

            return members;
        }

        private async Task<(bool IsPresent, string Location)> GetUserDayStatus(User user, DateTime date)
        {
            // Logique identique à celle existante
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

            var vacation = user.Vacations.FirstOrDefault(v =>
                date.Date >= v.StartDate.Date &&
                date.Date <= v.EndDate.Date);

            if (vacation != null)
            {
                return (false, "congés");
            }

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                return (false, "week-end");
            }

            var dayOfWeek = (int)date.DayOfWeek;
            var weeklySchedule = user.WeeklySchedules.FirstOrDefault(w => w.WeekDayId == dayOfWeek);

            if (weeklySchedule != null)
            {
                return (true, weeklySchedule.IsOnsite ? "sur site" : "télétravail");
            }

            return (true, "sur site");
        }
    }
}