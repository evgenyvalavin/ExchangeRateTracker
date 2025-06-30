using TrueCodeTestTask.Common.Models;

namespace TrueCodeTestTask.FinanceService.Interfaces;

public interface ICurrencyRepository
{
    Task<List<Currency>> GetAllAsync();
    Task<Currency?> GetByIdAsync(int id);
    Task<Currency?> GetByNameAsync(string name);
    Task<Currency> CreateAsync(Currency currency);
    Task<Currency> UpdateAsync(Currency currency);
    Task DeleteAsync(int id);
    Task BulkUpdateAsync(List<Currency> currencies);
}
