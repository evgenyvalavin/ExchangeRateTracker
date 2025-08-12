using ExchangeRateTracker.Common.DTOs;
using ExchangeRateTracker.Common.Models;

namespace ExchangeRateTracker.UserService.Interfaces;

public interface IAuthService
{
    Task<User?> GetUserById(Guid userId);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> LogoutAsync(string token);
}
