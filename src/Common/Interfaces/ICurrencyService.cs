using TrueCodeTestTask.Common.DTOs;

namespace TrueCodeTestTask.Common.Interfaces;

public interface ICurrencyService
{
    Task<CurrencyResponse> GetAllCurrenciesAsync();
    Task<CurrencyResponse> GetUserFavoriteCurrenciesAsync(int userId);
    Task<bool> AddFavoriteCurrencyAsync(int userId, int currencyId);
    Task<bool> RemoveFavoriteCurrencyAsync(int userId, int currencyId);
}
