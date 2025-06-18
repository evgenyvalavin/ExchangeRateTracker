using TrueCodeTestTask.Common.DTOs;
using TrueCodeTestTask.Common.Interfaces;
using TrueCodeTestTask.Common.Models;

namespace TrueCodeTestTask.UserService.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Check if user already exists
            if (await _userRepository.ExistsAsync(request.Name))
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
                Password = _passwordService.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);

            // Generate JWT token
            var token = _jwtService.GenerateToken(createdUser.Id, createdUser.Name);

            _logger.LogInformation("User {UserName} registered successfully", request.Name);

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
            _logger.LogError(ex, "Error registering user {UserName}", request.Name);
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
            var user = await _userRepository.GetByNameAsync(request.Name);
            if (user == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            if (!_passwordService.VerifyPassword(request.Password, user.Password))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user.Id, user.Name);

            _logger.LogInformation("User {UserName} logged in successfully", request.Name);

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
            _logger.LogError(ex, "Error logging in user {UserName}", request.Name);
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
            _logger.LogInformation("User logged out successfully");

            return new AuthResponse
            {
                Success = true,
                Message = "Logout successful"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return new AuthResponse
            {
                Success = false,
                Message = "An error occurred during logout"
            };
        }
    }
}
