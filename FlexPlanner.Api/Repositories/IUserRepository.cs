using FlexPlanner.Api.Models;

namespace FlexPlanner.Api.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<List<User>> GetUsersByTeamIdAsync(Guid teamId);
        Task UpdateUserAsync(User user);
    }
}
