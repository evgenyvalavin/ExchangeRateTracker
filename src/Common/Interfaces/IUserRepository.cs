using TrueCodeTestTask.Common.Models;

namespace TrueCodeTestTask.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByNameAsync(string name);
    Task<User> CreateAsync(User user);
    Task<bool> ExistsAsync(string name);
    Task<List<Currency>> GetFavoriteCurrenciesAsync(Guid userId);
    Task AddFavoriteCurrencyAsync(Guid userId, int currencyId);
    Task RemoveFavoriteCurrencyAsync(Guid userId, int currencyId);
}
