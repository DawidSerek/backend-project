using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.Interfaces.Services;

public interface IAdminService
{
    Task<List<AdminUserDto>> GetAllUsersAsync();
    Task<AdminUserDto?> GetUserAsync(string id);
    Task<IdentityResult> CreateUserAsync(CreateUserDto dto, string password);
    Task<IdentityResult> UpdateUserAsync(string id, UpdateUserDto dto);
    Task<IdentityResult> DeactivateUserAsync(string id);
    Task<IdentityResult> ActivateUserAsync(string id);
    Task<IdentityResult> AssignRoleAsync(string userId, string role);
    Task<IdentityResult> RemoveRoleAsync(string userId, string role);
    Task<List<string>> GetUserRolesAsync(string userId);
}

public record AdminUserDto(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string Department,
    string Status,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? DeactivatedAt,
    List<string> Roles
);

public record CreateUserDto(string Email, string FirstName, string LastName, string[] Roles);
public record UpdateUserDto(string? FirstName, string? LastName, string? Email);

