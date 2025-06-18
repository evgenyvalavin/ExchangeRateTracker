using Microsoft.EntityFrameworkCore;
using TrueCodeTestTask.Common.Models;

namespace TrueCodeTestTask.Common.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<UserCurrency> UserCurrencies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Currency entity
        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Rate).HasPrecision(18, 4);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure UserCurrency many-to-many relationship
        modelBuilder.Entity<UserCurrency>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.CurrencyId });

            entity.HasOne(e => e.User)
                .WithMany(e => e.FavoriteCurrencies)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Currency)
                .WithMany()
                .HasForeignKey(e => e.CurrencyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed some initial currencies
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Currency>().HasData(
            new Currency { Id = 1, Name = "USD", Rate = 90.0m, UpdatedAt = seedDate },
            new Currency { Id = 2, Name = "EUR", Rate = 100.0m, UpdatedAt = seedDate },
            new Currency { Id = 3, Name = "GBP", Rate = 115.0m, UpdatedAt = seedDate }
        );
    }
}
