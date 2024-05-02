namespace Backend.Services.Authentication;

public record AuthResult(
    bool Success,
    string Email,
    string UserName,
    string Token,
    string Role)
{
    public readonly Dictionary<string, string> ErrorMessages = new();
}