using FlexPlanner.Api.DTOs;
using FlexPlanner.Api.Models;
using FlexPlanner.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlexPlanner.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(user);
        }

        [HttpPut("me")]
        public async Task<ActionResult<UserDto>> UpdateCurrentUser(UpdateUserRequest request)
        {
            var userId = GetCurrentUserId();
            var user = await _userService.UpdateUserAsync(userId, request);
            return Ok(user);
        }

        [HttpGet("team/{teamId}/members")]
        public async Task<ActionResult<List<TeamMemberPresenceDto>>> GetTeamMembers(Guid teamId)
        {
            var members = await _userService.GetTeamMembersAsync(teamId);
            return Ok(members);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
        }
    }
}
