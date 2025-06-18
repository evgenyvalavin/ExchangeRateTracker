using Microsoft.EntityFrameworkCore;
using TrueCodeTestTask.Common.Data;
using TrueCodeTestTask.Common.Interfaces;
using TrueCodeTestTask.Common.Models;

namespace TrueCodeTestTask.FinanceService.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly ApplicationDbContext _context;

    public CurrencyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Currency>> GetAllAsync()
    {
        return await _context.Currencies
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Currency?> GetByIdAsync(int id)
    {
        return await _context.Currencies.FindAsync(id);
    }

    public async Task<Currency?> GetByNameAsync(string name)
    {
        return await _context.Currencies
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<Currency> CreateAsync(Currency currency)
    {
        _context.Currencies.Add(currency);
        await _context.SaveChangesAsync();
        return currency;
    }

    public async Task<Currency> UpdateAsync(Currency currency)
    {
        _context.Currencies.Update(currency);
        await _context.SaveChangesAsync();
        return currency;
    }

    public async Task DeleteAsync(int id)
    {
        var currency = await _context.Currencies.FindAsync(id);
        if (currency != null)
        {
            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();
        }
    }

    public async Task BulkUpdateAsync(List<Currency> currencies)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var currency in currencies)
            {
                var existingCurrency = await _context.Currencies
                    .FirstOrDefaultAsync(c => c.Name == currency.Name);

                if (existingCurrency != null)
                {
                    existingCurrency.Rate = currency.Rate;
                    existingCurrency.UpdatedAt = currency.UpdatedAt;
                }
                else
                {
                    _context.Currencies.Add(currency);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
