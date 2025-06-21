using System.ComponentModel.DataAnnotations;

namespace TrueCodeTestTask.Common.Models;

public class User
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property for user's favorite currencies
    public virtual ICollection<UserCurrency> FavoriteCurrencies { get; set; } = [];
}
