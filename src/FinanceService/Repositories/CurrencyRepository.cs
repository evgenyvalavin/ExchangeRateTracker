using Microsoft.EntityFrameworkCore;
using TrueCodeTestTask.Common.Data;
using TrueCodeTestTask.Common.Interfaces;
using TrueCodeTestTask.Common.Models;

namespace TrueCodeTestTask.FinanceService.Repositories;

public class CurrencyRepository(ApplicationDbContext context) : ICurrencyRepository
{

    public async Task<List<Currency>> GetAllAsync()
    {
        return await context.Currencies
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Currency?> GetByIdAsync(int id)
    {
        return await context.Currencies.FindAsync(id);
    }

    public async Task<Currency?> GetByNameAsync(string name)
    {
        return await context.Currencies
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<Currency> CreateAsync(Currency currency)
    {
        context.Currencies.Add(currency);
        await context.SaveChangesAsync();
        return currency;
    }

    public async Task<Currency> UpdateAsync(Currency currency)
    {
        context.Currencies.Update(currency);
        await context.SaveChangesAsync();
        return currency;
    }

    public async Task DeleteAsync(int id)
    {
        var currency = await context.Currencies.FindAsync(id);
        if (currency != null)
        {
            context.Currencies.Remove(currency);
            await context.SaveChangesAsync();
        }
    }

    public async Task BulkUpdateAsync(List<Currency> currencies)
    {
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            foreach (var currency in currencies)
            {
                var existingCurrency = await context.Currencies
                    .FirstOrDefaultAsync(c => c.Name == currency.Name);

                if (existingCurrency != null)
                {
                    existingCurrency.Rate = currency.Rate;
                    existingCurrency.UpdatedAt = currency.UpdatedAt;
                }
                else
                {
                    context.Currencies.Add(currency);
                }
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
