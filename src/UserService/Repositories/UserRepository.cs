using Microsoft.EntityFrameworkCore;
using TrueCodeTestTask.Common.Data;
using TrueCodeTestTask.Common.Interfaces;
using TrueCodeTestTask.Common.Models;

namespace TrueCodeTestTask.UserService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.FavoriteCurrencies)
            .ThenInclude(uc => uc.Currency)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByNameAsync(string name)
    {
        return await _context.Users
            .Include(u => u.FavoriteCurrencies)
            .ThenInclude(uc => uc.Currency)
            .FirstOrDefaultAsync(u => u.Name == name);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ExistsAsync(string name)
    {
        return await _context.Users.AnyAsync(u => u.Name == name);
    }

    public async Task<List<Currency>> GetFavoriteCurrenciesAsync(int userId)
    {
        return await _context.UserCurrencies
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

        _context.UserCurrencies.Add(userCurrency);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveFavoriteCurrencyAsync(int userId, int currencyId)
    {
        var userCurrency = await _context.UserCurrencies
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CurrencyId == currencyId);

        if (userCurrency != null)
        {
            _context.UserCurrencies.Remove(userCurrency);
            await _context.SaveChangesAsync();
        }
    }
}
