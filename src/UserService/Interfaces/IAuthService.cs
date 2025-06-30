using TrueCodeTestTask.Common.DTOs;
using TrueCodeTestTask.Common.Models;

namespace TrueCodeTestTask.UserService.Interfaces;

public interface IAuthService
{
    Task<User?> GetUserById(Guid userId);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> LogoutAsync(string token);
}
