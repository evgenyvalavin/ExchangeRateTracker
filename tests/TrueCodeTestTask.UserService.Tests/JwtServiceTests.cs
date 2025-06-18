using Microsoft.Extensions.Configuration;
using Moq;
using TrueCodeTestTask.UserService.Services;

namespace TrueCodeTestTask.UserService.Tests;

public class JwtServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        
        // Setup configuration values
        _mockConfiguration.Setup(x => x["JWT:SecretKey"])
            .Returns("your-super-secret-jwt-key-here-make-it-long-and-secure");
        _mockConfiguration.Setup(x => x["JWT:Issuer"])
            .Returns("TrueCodeTestTask");
        _mockConfiguration.Setup(x => x["JWT:Audience"])
            .Returns("TrueCodeTestTask");

        _jwtService = new JwtService(_mockConfiguration.Object);
    }

    [Fact]
    public void GenerateToken_WithValidInput_ReturnsValidToken()
    {
        // Arrange
        var userId = 1;
        var userName = "testuser";

        // Act
        var token = _jwtService.GenerateToken(userId, userName);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.Contains(".", token); // JWT tokens contain dots
    }

    [Fact]
    public void ValidateToken_WithValidToken_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var userName = "testuser";
        var token = _jwtService.GenerateToken(userId, userName);

        // Act
        var isValid = _jwtService.ValidateToken(token);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ReturnsFalse()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var isValid = _jwtService.ValidateToken(invalidToken);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void GetUserIdFromToken_WithValidToken_ReturnsCorrectUserId()
    {
        // Arrange
        var userId = 123;
        var userName = "testuser";
        var token = _jwtService.GenerateToken(userId, userName);

        // Act
        var extractedUserId = _jwtService.GetUserIdFromToken(token);

        // Assert
        Assert.Equal(userId, extractedUserId);
    }

    [Fact]
    public void GetUserNameFromToken_WithValidToken_ReturnsCorrectUserName()
    {
        // Arrange
        var userId = 1;
        var userName = "testuser";
        var token = _jwtService.GenerateToken(userId, userName);

        // Act
        var extractedUserName = _jwtService.GetUserNameFromToken(token);

        // Assert
        Assert.Equal(userName, extractedUserName);
    }

    [Fact]
    public void GetUserIdFromToken_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var userId = _jwtService.GetUserIdFromToken(invalidToken);

        // Assert
        Assert.Null(userId);
    }

    [Fact]
    public void GetUserNameFromToken_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var userName = _jwtService.GetUserNameFromToken(invalidToken);

        // Assert
        Assert.Null(userName);
    }
}
