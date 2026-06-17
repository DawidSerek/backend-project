namespace ApplicationCore.Models.Dto;

public record LoginDto(string Email, string Password);

public record RefreshTokenDto(string AccessToken, string RefreshToken);

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User);

public record UserDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Department,
    string Status,
    IEnumerable<string> Roles);

public record RevokeTokenRequest(string Token);