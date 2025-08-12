using System.ComponentModel.DataAnnotations;

namespace ExchangeRateTracker.Common.DTOs;

public class LoginRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}
