using Microsoft.Extensions.Logging;
using Moq;
using TrueCodeTestTask.Common.DTOs;
using TrueCodeTestTask.Common.Interfaces;
using TrueCodeTestTask.Common.Models;
using TrueCodeTestTask.UserService.Services;

namespace TrueCodeTestTask.UserService.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordService> _mockPasswordService;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordService = new Mock<IPasswordService>();
        _mockJwtService = new Mock<IJwtService>();
        _mockLogger = new Mock<ILogger<AuthService>>();

        _authService = new AuthService(
            _mockUserRepository.Object,
            _mockPasswordService.Object,
            _mockJwtService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Name = "testuser",
            Password = "password123"
        };

        _mockUserRepository.Setup(x => x.ExistsAsync(request.Name))
            .ReturnsAsync(false);

        _mockPasswordService.Setup(x => x.HashPassword(request.Password))
            .Returns("hashedpassword");

        var createdUser = new User
        {
            Id = 1,
            Name = request.Name,
            Password = "hashedpassword",
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(createdUser);

        _mockJwtService.Setup(x => x.GenerateToken(createdUser.Id, createdUser.Name))
            .Returns("jwt-token");

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("User registered successfully", result.Message);
        Assert.Equal("jwt-token", result.Token);
        Assert.NotNull(result.User);
        Assert.Equal(createdUser.Id, result.User.Id);
        Assert.Equal(createdUser.Name, result.User.Name);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUser_ReturnsFailureResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Name = "existinguser",
            Password = "password123"
        };

        _mockUserRepository.Setup(x => x.ExistsAsync(request.Name))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User with this name already exists", result.Message);
        Assert.Null(result.Token);
        Assert.Null(result.User);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Name = "testuser",
            Password = "password123"
        };

        var user = new User
        {
            Id = 1,
            Name = request.Name,
            Password = "hashedpassword",
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(x => x.GetByNameAsync(request.Name))
            .ReturnsAsync(user);

        _mockPasswordService.Setup(x => x.VerifyPassword(request.Password, user.Password))
            .Returns(true);

        _mockJwtService.Setup(x => x.GenerateToken(user.Id, user.Name))
            .Returns("jwt-token");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Login successful", result.Message);
        Assert.Equal("jwt-token", result.Token);
        Assert.NotNull(result.User);
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal(user.Name, result.User.Name);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidUser_ReturnsFailureResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Name = "nonexistentuser",
            Password = "password123"
        };

        _mockUserRepository.Setup(x => x.GetByNameAsync(request.Name))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid username or password", result.Message);
        Assert.Null(result.Token);
        Assert.Null(result.User);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsFailureResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Name = "testuser",
            Password = "wrongpassword"
        };

        var user = new User
        {
            Id = 1,
            Name = request.Name,
            Password = "hashedpassword",
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(x => x.GetByNameAsync(request.Name))
            .ReturnsAsync(user);

        _mockPasswordService.Setup(x => x.VerifyPassword(request.Password, user.Password))
            .Returns(false);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid username or password", result.Message);
        Assert.Null(result.Token);
        Assert.Null(result.User);
    }
}
