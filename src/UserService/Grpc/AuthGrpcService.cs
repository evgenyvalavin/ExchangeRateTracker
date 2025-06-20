using Grpc.Core;
using TrueCodeTestTask.Common.DTOs;
using TrueCodeTestTask.Common.Grpc;
using TrueCodeTestTask.Common.Interfaces;

namespace TrueCodeTestTask.UserService.Grpc;

public class AuthGrpcService(IAuthService authService, IJwtService jwtService, ILogger<AuthGrpcService> logger) : AuthService.AuthServiceBase
{

    public override async Task<Common.Grpc.AuthResponse> Register(Common.Grpc.RegisterRequest request, ServerCallContext context)
    {
        try
        {
            var registerDto = new Common.DTOs.RegisterRequest
            {
                Name = request.Name,
                Password = request.Password
            };

            var result = await authService.RegisterAsync(registerDto);

            return new Common.Grpc.AuthResponse
            {
                Success = result.Success,
                Message = result.Message,
                Token = result.Token ?? string.Empty,
                User = result.User != null ? new UserInfo
                {
                    Id = result.User.Id,
                    Name = result.User.Name,
                    CreatedAt = result.User.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                } : null
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in gRPC Register method");
            return new Common.Grpc.AuthResponse
            {
                Success = false,
                Message = "Internal server error"
            };
        }
    }

    public override async Task<Common.Grpc.AuthResponse> Login(Common.Grpc.LoginRequest request, ServerCallContext context)
    {
        try
        {
            var loginDto = new Common.DTOs.LoginRequest
            {
                Name = request.Name,
                Password = request.Password
            };

            var result = await authService.LoginAsync(loginDto);

            return new Common.Grpc.AuthResponse
            {
                Success = result.Success,
                Message = result.Message,
                Token = result.Token ?? string.Empty,
                User = result.User != null ? new UserInfo
                {
                    Id = result.User.Id,
                    Name = result.User.Name,
                    CreatedAt = result.User.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                } : null
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in gRPC Login method");
            return new Common.Grpc.AuthResponse
            {
                Success = false,
                Message = "Internal server error"
            };
        }
    }

    public override Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
    {
        try
        {
            var isValid = jwtService.ValidateToken(request.Token);
            var userId = jwtService.GetUserIdFromToken(request.Token);
            var userName = jwtService.GetUserNameFromToken(request.Token);

            return Task.FromResult(new ValidateTokenResponse
            {
                IsValid = isValid,
                UserId = userId ?? 0,
                UserName = userName ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in gRPC ValidateToken method");
            return Task.FromResult(new ValidateTokenResponse
            {
                IsValid = false,
                UserId = 0,
                UserName = string.Empty
            });
        }
    }

    public override Task<UserInfoResponse> GetUserInfo(GetUserInfoRequest request, ServerCallContext context)
    {
        try
        {
            var isValid = jwtService.ValidateToken(request.Token);
            if (!isValid)
            {
                return Task.FromResult(new UserInfoResponse
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var userId = jwtService.GetUserIdFromToken(request.Token);
            var userName = jwtService.GetUserNameFromToken(request.Token);

            if (userId.HasValue && !string.IsNullOrEmpty(userName))
            {
                return Task.FromResult(new UserInfoResponse
                {
                    Success = true,
                    Message = "User info retrieved successfully",
                    User = new UserInfo
                    {
                        Id = userId.Value,
                        Name = userName,
                        CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") // This would normally come from database
                    }
                });
            }

            return Task.FromResult(new UserInfoResponse
            {
                Success = false,
                Message = "User not found"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in gRPC GetUserInfo method");
            return Task.FromResult(new UserInfoResponse
            {
                Success = false,
                Message = "Internal server error"
            });
        }
    }
}
