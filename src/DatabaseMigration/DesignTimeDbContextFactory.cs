using ExchangeRateTracker.Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExchangeRateTracker.DatabaseMigration;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=exchangeratetrackerdb;Username=postgres;Password=postgres123";

        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("ExchangeRateTracker.DatabaseMigration"));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
