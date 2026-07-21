namespace AssetTracking.Application.DTOs.Auth;

public record LoginRequest(string Email, string Password);

public record LoginResponse(
    string AccessToken,
    DateTime ExpiresAt,
    UserDto User);

public record UserDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    IReadOnlyList<string> Permissions);
