using System.Text;
using System.Xml.Linq;
using TrueCodeTestTask.Common.Data;
using TrueCodeTestTask.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace TrueCodeTestTask.CurrencyService.Services;

public class CbrCurrencyService
{
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CbrCurrencyService> _logger;
    private const string CbrUrl = "http://www.cbr.ru/scripts/XML_daily.asp";

    public CbrCurrencyService(HttpClient httpClient, ApplicationDbContext context, ILogger<CbrCurrencyService> logger)
    {
        _httpClient = httpClient;
        _context = context;
        _logger = logger;
    }

    public async Task UpdateCurrencyRatesAsync()
    {
        try
        {
            _logger.LogInformation("Starting currency rates update from CBR...");

            // CBR returns data in windows-1251 encoding, need to handle it properly
            var responseBytes = await _httpClient.GetByteArrayAsync(CbrUrl);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var encoding = Encoding.GetEncoding("windows-1251");
            var response = encoding.GetString(responseBytes);
            var currencies = ParseCbrXml(response);

            if (currencies.Any())
            {
                await UpdateDatabaseAsync(currencies);
                _logger.LogInformation("Successfully updated {Count} currency rates", currencies.Count);
            }
            else
            {
                _logger.LogWarning("No currencies found in CBR response");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating currency rates from CBR");
            throw;
        }
    }

    private List<Currency> ParseCbrXml(string xmlContent)
    {
        var currencies = new List<Currency>();

        try
        {
            var doc = XDocument.Parse(xmlContent);
            var valuteElements = doc.Descendants("Valute");

            foreach (var valute in valuteElements)
            {
                var charCode = valute.Element("CharCode")?.Value;
                var name = valute.Element("Name")?.Value;
                var valueStr = valute.Element("Value")?.Value?.Replace(',', '.');
                var nominalStr = valute.Element("Nominal")?.Value;

                if (string.IsNullOrEmpty(charCode) || string.IsNullOrEmpty(name) ||
                    string.IsNullOrEmpty(valueStr) || string.IsNullOrEmpty(nominalStr))
                    continue;

                if (decimal.TryParse(valueStr, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out var value) &&
                    int.TryParse(nominalStr, out var nominal))
                {
                    // Calculate rate per 1 unit of currency
                    var rate = nominal > 1 ? value / nominal : value;

                    currencies.Add(new Currency
                    {
                        Name = charCode,
                        Rate = rate,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing CBR XML response");
            throw;
        }

        return currencies;
    }

    private async Task UpdateDatabaseAsync(List<Currency> currencies)
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
