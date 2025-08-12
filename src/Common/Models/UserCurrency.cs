namespace ExchangeRateTracker.Common.Models;

public class UserCurrency
{
    public Guid UserId { get; set; }
    public int CurrencyId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Currency Currency { get; set; } = null!;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
