using TrueCodeTestTask.Common.DTOs;

namespace TrueCodeTestTask.FinanceService.Interfaces;

public interface ICurrencyService
{
    Task<CurrencyResponse> GetAllCurrenciesAsync();
    Task<CurrencyResponse> GetUserFavoriteCurrenciesAsync(Guid userId);
    Task<bool> AddFavoriteCurrencyAsync(Guid userId, int currencyId);
    Task<bool> RemoveFavoriteCurrencyAsync(Guid userId, int currencyId);
}
