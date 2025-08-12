using ExchangeRateTracker.Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// Build configuration
var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

// Build service provider
var services = new ServiceCollection();

services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? "Host=localhost;Database=exchangeratetrackerdb;Username=postgres;Password=postgres123";
    options.UseNpgsql(connectionString, b => b.MigrationsAssembly("ExchangeRateTracker.DatabaseMigration"));
});

services.AddLogging(builder => builder.AddConsole());

var serviceProvider = services.BuildServiceProvider();

// Get logger
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Starting database migration...");

    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Apply migrations to create/update database schema
    await context.Database.MigrateAsync();
    logger.LogInformation("Database migration completed successfully.");
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during database migration.");
    Environment.Exit(1);
}
