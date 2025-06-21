using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TrueCodeTestTask.Common.Data;

namespace TrueCodeTestTask.DatabaseMigration;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=truecodedb;Username=postgres;Password=postgres123";

        optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("TrueCodeTestTask.DatabaseMigration"));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
