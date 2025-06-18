using TrueCodeTestTask.Common.Models;

namespace TrueCodeTestTask.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByNameAsync(string name);
    Task<User> CreateAsync(User user);
    Task<bool> ExistsAsync(string name);
    Task<List<Currency>> GetFavoriteCurrenciesAsync(int userId);
    Task AddFavoriteCurrencyAsync(int userId, int currencyId);
    Task RemoveFavoriteCurrencyAsync(int userId, int currencyId);
}
