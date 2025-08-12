using ExchangeRateTracker.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRateTracker.Common.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
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
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
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
    }
}
