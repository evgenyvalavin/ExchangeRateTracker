using ExchangeRateTracker.Common.DTOs;
using ExchangeRateTracker.Common.Interfaces;
using ExchangeRateTracker.Common.Models;
using ExchangeRateTracker.UserService.Interfaces;

namespace ExchangeRateTracker.UserService.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordService passwordService,
    IJwtService jwtService,
    ILogger<AuthService> logger) : IAuthService
{
    public Task<User?> GetUserById(Guid userId)
    {
        return userRepository.GetByIdAsync(userId);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Check if user already exists
            if (await userRepository.ExistsAsync(request.Name))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "User with this name already exists"
                };
            }

            // Create new user
            var user = new User
            {
                Name = request.Name,
                Password = passwordService.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await userRepository.CreateAsync(user);

            // Generate JWT token
            var token = jwtService.GenerateToken(createdUser.Id, createdUser.Name);

            logger.LogInformation("User {UserName} registered successfully", request.Name);

            return new AuthResponse
            {
                Success = true,
                Message = "User registered successfully",
                Token = token,
                User = new UserDto
                {
                    Id = createdUser.Id,
                    Name = createdUser.Name,
                    CreatedAt = createdUser.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering user {UserName}", request.Name);
            return new AuthResponse
            {
                Success = false,
                Message = "An error occurred during registration"
            };
        }
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await userRepository.GetByNameAsync(request.Name);
            if (user == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            if (!passwordService.VerifyPassword(request.Password, user.Password))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            // Generate JWT token
            var token = jwtService.GenerateToken(user.Id, user.Name);

            logger.LogInformation("User {UserName} logged in successfully", request.Name);

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    CreatedAt = user.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error logging in user {UserName}", request.Name);
            return new AuthResponse
            {
                Success = false,
                Message = "An error occurred during login"
            };
        }
    }

    public async Task<AuthResponse> LogoutAsync(string token)
    {
        try
        {
            // In a real application, you might want to blacklist the token
            // For now, we'll just return success
            logger.LogInformation("User logged out successfully");

            return new AuthResponse
            {
                Success = true,
                Message = "Logout successful"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during logout");
            return new AuthResponse
            {
                Success = false,
                Message = "An error occurred during logout"
            };
        }
    }
}
