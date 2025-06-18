using Microsoft.Extensions.Logging;
using Moq;
using TrueCodeTestTask.Common.Interfaces;
using TrueCodeTestTask.Common.Models;
using TrueCodeTestTask.FinanceService.Services;

namespace TrueCodeTestTask.FinanceService.Tests;

public class CurrencyServiceTests
{
    private readonly Mock<ICurrencyRepository> _mockCurrencyRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<CurrencyService>> _mockLogger;
    private readonly CurrencyService _currencyService;

    public CurrencyServiceTests()
    {
        _mockCurrencyRepository = new Mock<ICurrencyRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<CurrencyService>>();

        _currencyService = new CurrencyService(
            _mockCurrencyRepository.Object,
            _mockUserRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllCurrenciesAsync_WithValidData_ReturnsSuccessResponse()
    {
        // Arrange
        var currencies = new List<Currency>
        {
            new Currency { Id = 1, Name = "USD", Rate = 90.0m, UpdatedAt = DateTime.UtcNow },
            new Currency { Id = 2, Name = "EUR", Rate = 100.0m, UpdatedAt = DateTime.UtcNow }
        };

        _mockCurrencyRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(currencies);

        // Act
        var result = await _currencyService.GetAllCurrenciesAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Currencies retrieved successfully", result.Message);
        Assert.Equal(2, result.Currencies.Count);
        Assert.Equal("USD", result.Currencies[0].Name);
        Assert.Equal("EUR", result.Currencies[1].Name);
    }

    [Fact]
    public async Task GetUserFavoriteCurrenciesAsync_WithValidUserId_ReturnsSuccessResponse()
    {
        // Arrange
        var userId = 1;
        var favoriteCurrencies = new List<Currency>
        {
            new Currency { Id = 1, Name = "USD", Rate = 90.0m, UpdatedAt = DateTime.UtcNow }
        };

        _mockUserRepository.Setup(x => x.GetFavoriteCurrenciesAsync(userId))
            .ReturnsAsync(favoriteCurrencies);

        // Act
        var result = await _currencyService.GetUserFavoriteCurrenciesAsync(userId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Favorite currencies retrieved successfully", result.Message);
        Assert.Single(result.Currencies);
        Assert.Equal("USD", result.Currencies[0].Name);
    }

    [Fact]
    public async Task AddFavoriteCurrencyAsync_WithValidData_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var currencyId = 1;
        var currency = new Currency { Id = currencyId, Name = "USD", Rate = 90.0m };
        var user = new User { Id = userId, Name = "testuser" };

        _mockCurrencyRepository.Setup(x => x.GetByIdAsync(currencyId))
            .ReturnsAsync(currency);
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.AddFavoriteCurrencyAsync(userId, currencyId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _currencyService.AddFavoriteCurrencyAsync(userId, currencyId);

        // Assert
        Assert.True(result);
        _mockUserRepository.Verify(x => x.AddFavoriteCurrencyAsync(userId, currencyId), Times.Once);
    }

    [Fact]
    public async Task AddFavoriteCurrencyAsync_WithInvalidCurrency_ReturnsFalse()
    {
        // Arrange
        var userId = 1;
        var currencyId = 999;

        _mockCurrencyRepository.Setup(x => x.GetByIdAsync(currencyId))
            .ReturnsAsync((Currency?)null);

        // Act
        var result = await _currencyService.AddFavoriteCurrencyAsync(userId, currencyId);

        // Assert
        Assert.False(result);
        _mockUserRepository.Verify(x => x.AddFavoriteCurrencyAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task AddFavoriteCurrencyAsync_WithInvalidUser_ReturnsFalse()
    {
        // Arrange
        var userId = 999;
        var currencyId = 1;
        var currency = new Currency { Id = currencyId, Name = "USD", Rate = 90.0m };

        _mockCurrencyRepository.Setup(x => x.GetByIdAsync(currencyId))
            .ReturnsAsync(currency);
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _currencyService.AddFavoriteCurrencyAsync(userId, currencyId);

        // Assert
        Assert.False(result);
        _mockUserRepository.Verify(x => x.AddFavoriteCurrencyAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task RemoveFavoriteCurrencyAsync_WithValidData_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var currencyId = 1;

        _mockUserRepository.Setup(x => x.RemoveFavoriteCurrencyAsync(userId, currencyId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _currencyService.RemoveFavoriteCurrencyAsync(userId, currencyId);

        // Assert
        Assert.True(result);
        _mockUserRepository.Verify(x => x.RemoveFavoriteCurrencyAsync(userId, currencyId), Times.Once);
    }
}
