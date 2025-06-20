using TrueCodeTestTask.Common.DTOs;
using TrueCodeTestTask.Common.Interfaces;

namespace TrueCodeTestTask.FinanceService.Services;

public class CurrencyService(
    ICurrencyRepository currencyRepository,
    IUserRepository userRepository,
    ILogger<CurrencyService> logger) : ICurrencyService
{

    public async Task<CurrencyResponse> GetAllCurrenciesAsync()
    {
        try
        {
            var currencies = await currencyRepository.GetAllAsync();

            return new CurrencyResponse
            {
                Success = true,
                Message = "Currencies retrieved successfully",
                Currencies = [.. currencies.Select(c => new CurrencyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Rate = c.Rate,
                    UpdatedAt = c.UpdatedAt
                })]
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving currencies");
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
            var favoriteCurrencies = await userRepository.GetFavoriteCurrenciesAsync(userId);

            return new CurrencyResponse
            {
                Success = true,
                Message = "Favorite currencies retrieved successfully",
                Currencies = [.. favoriteCurrencies.Select(c => new CurrencyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Rate = c.Rate,
                    UpdatedAt = c.UpdatedAt
                })]
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving favorite currencies for user {UserId}", userId);
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
            var currency = await currencyRepository.GetByIdAsync(currencyId);
            if (currency == null)
            {
                logger.LogWarning("Currency with ID {CurrencyId} not found", currencyId);
                return false;
            }

            // Check if user exists
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning("User with ID {UserId} not found", userId);
                return false;
            }

            await userRepository.AddFavoriteCurrencyAsync(userId, currencyId);
            logger.LogInformation("Added currency {CurrencyId} to favorites for user {UserId}", currencyId, userId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding favorite currency {CurrencyId} for user {UserId}", currencyId, userId);
            return false;
        }
    }

    public async Task<bool> RemoveFavoriteCurrencyAsync(int userId, int currencyId)
    {
        try
        {
            await userRepository.RemoveFavoriteCurrencyAsync(userId, currencyId);
            logger.LogInformation("Removed currency {CurrencyId} from favorites for user {UserId}", currencyId, userId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing favorite currency {CurrencyId} for user {UserId}", currencyId, userId);
            return false;
        }
    }
}
