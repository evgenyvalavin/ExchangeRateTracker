using System.ComponentModel.DataAnnotations;

namespace TrueCodeTestTask.Common.Models;

public class Currency
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public decimal Rate { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
