using FlexPlanner.Api.DTOs;

namespace FlexPlanner.Api.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
