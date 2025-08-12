using ExchangeRateTracker.Common.Data;
using ExchangeRateTracker.Common.Models;
using ExchangeRateTracker.UserService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRateTracker.UserService.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Users
            .Include(u => u.FavoriteCurrencies)
            .ThenInclude(uc => uc.Currency)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByNameAsync(string name)
    {
        return await context.Users
            .Include(u => u.FavoriteCurrencies)
            .ThenInclude(uc => uc.Currency)
            .FirstOrDefaultAsync(u => u.Name == name);
    }

    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ExistsAsync(string name)
    {
        return await context.Users.AnyAsync(u => u.Name == name);
    }

    public async Task<List<Currency>> GetFavoriteCurrenciesAsync(Guid userId)
    {
        return await context.UserCurrencies
            .Where(uc => uc.UserId == userId)
            .Select(uc => uc.Currency)
            .ToListAsync();
    }

    public async Task AddFavoriteCurrencyAsync(Guid userId, int currencyId)
    {
        var userCurrency = new UserCurrency
        {
            UserId = userId,
            CurrencyId = currencyId,
            AddedAt = DateTime.UtcNow
        };

        context.UserCurrencies.Add(userCurrency);
        await context.SaveChangesAsync();
    }

    public async Task RemoveFavoriteCurrencyAsync(Guid userId, int currencyId)
    {
        var userCurrency = await context.UserCurrencies
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CurrencyId == currencyId);

        if (userCurrency != null)
        {
            context.UserCurrencies.Remove(userCurrency);
            await context.SaveChangesAsync();
        }
    }
}
