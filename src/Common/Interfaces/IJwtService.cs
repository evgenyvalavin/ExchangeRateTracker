namespace TrueCodeTestTask.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(int userId, string userName);
    bool ValidateToken(string token);
    int? GetUserIdFromToken(string token);
    string? GetUserNameFromToken(string token);
}
