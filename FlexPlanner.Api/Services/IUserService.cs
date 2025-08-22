using FlexPlanner.Api.DTOs;

namespace FlexPlanner.Api.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequest request);
        Task<List<TeamMemberPresenceDto>> GetTeamMembersAsync(Guid teamId);
    }
}
