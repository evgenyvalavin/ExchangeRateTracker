using ExchangeRateTracker.Common.Data;
using ExchangeRateTracker.CurrencyService;
using ExchangeRateTracker.CurrencyService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// Add configuration
builder.Configuration.AddEnvironmentVariables();

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Host=localhost;Database=exchangeratetrackerdb;Username=postgres;Password=postgres123";
    options.UseNpgsql(connectionString);
});

// Add HTTP client
builder.Services.AddHttpClient<CbrCurrencyService>();

// Add services
builder.Services.AddScoped<CbrCurrencyService>();

// Add hosted service
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

// Database should be initialized by DatabaseMigration service

host.Run();
