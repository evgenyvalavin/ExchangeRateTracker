using ExchangeRateTracker.UserService.Services;

namespace ExchangeRateTracker.UserService.Tests;

public class PasswordServiceTests
{
    private readonly PasswordService _passwordService;

    public PasswordServiceTests()
    {
        _passwordService = new PasswordService();
    }

    [Fact]
    public void HashPassword_WithValidPassword_ReturnsHashedPassword()
    {
        // Arrange
        var password = "testpassword123";

        // Act
        var hashedPassword = _passwordService.HashPassword(password);

        // Assert
        Assert.NotNull(hashedPassword);
        Assert.NotEmpty(hashedPassword);
        Assert.NotEqual(password, hashedPassword);
        Assert.True(hashedPassword.Length > password.Length);
    }

    [Fact]
    public void HashPassword_WithSamePassword_ReturnsDifferentHashes()
    {
        // Arrange
        var password = "testpassword123";

        // Act
        var hash1 = _passwordService.HashPassword(password);
        var hash2 = _passwordService.HashPassword(password);

        // Assert
        Assert.NotEqual(hash1, hash2); // BCrypt uses salt, so hashes should be different
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "testpassword123";
        var hashedPassword = _passwordService.HashPassword(password);

        // Act
        var isValid = _passwordService.VerifyPassword(password, hashedPassword);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var password = "testpassword123";
        var wrongPassword = "wrongpassword";
        var hashedPassword = _passwordService.HashPassword(password);

        // Act
        var isValid = _passwordService.VerifyPassword(wrongPassword, hashedPassword);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void VerifyPassword_WithEmptyPassword_ReturnsFalse()
    {
        // Arrange
        var password = "testpassword123";
        var hashedPassword = _passwordService.HashPassword(password);

        // Act
        var isValid = _passwordService.VerifyPassword("", hashedPassword);

        // Assert
        Assert.False(isValid);
    }

    [Theory]
    [InlineData("password")]
    [InlineData("123456")]
    [InlineData("P@ssw0rd!")]
    [InlineData("very-long-password-with-special-characters-123!@#")]
    public void HashAndVerifyPassword_WithVariousPasswords_WorksCorrectly(string password)
    {
        // Act
        var hashedPassword = _passwordService.HashPassword(password);
        var isValid = _passwordService.VerifyPassword(password, hashedPassword);

        // Assert
        Assert.True(isValid);
    }
}
