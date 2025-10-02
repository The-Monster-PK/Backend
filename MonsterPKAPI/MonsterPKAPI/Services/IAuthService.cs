using MonsterPKAPI.DTOs;

namespace MonsterPKAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        string GenerateJwtToken(string userId, string username, string email);
    }
}
