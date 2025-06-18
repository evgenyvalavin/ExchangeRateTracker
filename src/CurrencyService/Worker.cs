using TrueCodeTestTask.CurrencyService.Services;

namespace TrueCodeTestTask.CurrencyService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _updateInterval = TimeSpan.FromHours(1); // Update every hour

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Currency Service Worker started");

        // Initial update
        await UpdateCurrencies();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_updateInterval, stoppingToken);
                await UpdateCurrencies();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Currency Service Worker is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Currency Service Worker");
                // Continue running even if there's an error
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task UpdateCurrencies()
    {
        using var scope = _serviceProvider.CreateScope();
        var currencyService = scope.ServiceProvider.GetRequiredService<CbrCurrencyService>();

        try
        {
            await currencyService.UpdateCurrencyRatesAsync();
            _logger.LogInformation("Currency rates updated successfully at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update currency rates");
        }
    }
}
