namespace ExchangeRateTracker.Common.DTOs;

public class CurrencyResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<CurrencyDto> Currencies { get; set; } = [];
}
