using Microsoft.EntityFrameworkCore;
using TrueCodeTestTask.Common.Data;
using TrueCodeTestTask.Common.Interfaces;
using TrueCodeTestTask.Common.Models;

namespace TrueCodeTestTask.FinanceService.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{

    public async Task<User?> GetByIdAsync(int id)
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

    public async Task<List<Currency>> GetFavoriteCurrenciesAsync(int userId)
    {
        return await context.UserCurrencies
            .Where(uc => uc.UserId == userId)
            .Select(uc => uc.Currency)
            .ToListAsync();
    }

    public async Task AddFavoriteCurrencyAsync(int userId, int currencyId)
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

    public async Task RemoveFavoriteCurrencyAsync(int userId, int currencyId)
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
