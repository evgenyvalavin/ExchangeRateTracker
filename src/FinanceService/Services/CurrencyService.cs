using TrueCodeTestTask.Common.DTOs;
using TrueCodeTestTask.Common.Interfaces;

namespace TrueCodeTestTask.FinanceService.Services;

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CurrencyService> _logger;

    public CurrencyService(
        ICurrencyRepository currencyRepository,
        IUserRepository userRepository,
        ILogger<CurrencyService> logger)
    {
        _currencyRepository = currencyRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<CurrencyResponse> GetAllCurrenciesAsync()
    {
        try
        {
            var currencies = await _currencyRepository.GetAllAsync();
            
            return new CurrencyResponse
            {
                Success = true,
                Message = "Currencies retrieved successfully",
                Currencies = currencies.Select(c => new CurrencyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Rate = c.Rate,
                    UpdatedAt = c.UpdatedAt
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving currencies");
            return new CurrencyResponse
            {
                Success = false,
                Message = "An error occurred while retrieving currencies"
            };
        }
    }

    public async Task<CurrencyResponse> GetUserFavoriteCurrenciesAsync(int userId)
    {
        try
        {
            var favoriteCurrencies = await _userRepository.GetFavoriteCurrenciesAsync(userId);
            
            return new CurrencyResponse
            {
                Success = true,
                Message = "Favorite currencies retrieved successfully",
                Currencies = favoriteCurrencies.Select(c => new CurrencyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Rate = c.Rate,
                    UpdatedAt = c.UpdatedAt
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving favorite currencies for user {UserId}", userId);
            return new CurrencyResponse
            {
                Success = false,
                Message = "An error occurred while retrieving favorite currencies"
            };
        }
    }

    public async Task<bool> AddFavoriteCurrencyAsync(int userId, int currencyId)
    {
        try
        {
            // Check if currency exists
            var currency = await _currencyRepository.GetByIdAsync(currencyId);
            if (currency == null)
            {
                _logger.LogWarning("Currency with ID {CurrencyId} not found", currencyId);
                return false;
            }

            // Check if user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return false;
            }

            await _userRepository.AddFavoriteCurrencyAsync(userId, currencyId);
            _logger.LogInformation("Added currency {CurrencyId} to favorites for user {UserId}", currencyId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding favorite currency {CurrencyId} for user {UserId}", currencyId, userId);
            return false;
        }
    }

    public async Task<bool> RemoveFavoriteCurrencyAsync(int userId, int currencyId)
    {
        try
        {
            await _userRepository.RemoveFavoriteCurrencyAsync(userId, currencyId);
            _logger.LogInformation("Removed currency {CurrencyId} from favorites for user {UserId}", currencyId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing favorite currency {CurrencyId} for user {UserId}", currencyId, userId);
            return false;
        }
    }
}
