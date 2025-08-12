namespace ExchangeRateTracker.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid userId, string userName);
    bool ValidateToken(string token);
    Guid? GetUserIdFromToken(string token);
    string? GetUserNameFromToken(string token);
}
