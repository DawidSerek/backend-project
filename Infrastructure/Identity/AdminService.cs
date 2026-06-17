using ApplicationCore.Interfaces.Services;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class AdminService(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager) : IAdminService
{
    public async Task<List<AdminUserDto>> GetAllUsersAsync()
    {
        var users = userManager.Users.ToList();
        var result = new List<AdminUserDto>();
        foreach (var u in users)
        {
            var roles = await userManager.GetRolesAsync(u);
            result.Add(MapToDto(u, roles.ToList()));
        }
        return result;
    }

    public async Task<AdminUserDto?> GetUserAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return null;
        var roles = await userManager.GetRolesAsync(user);
        return MapToDto(user, roles.ToList());
    }

    public async Task<IdentityResult> CreateUserAsync(CreateUserDto dto, string password)
    {
        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Status = SystemUserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded) return result;

        foreach (var role in dto.Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
            await userManager.AddToRoleAsync(user, role);
        }

        return result;
    }

    public async Task<IdentityResult> UpdateUserAsync(string id, UpdateUserDto dto)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        if (!string.IsNullOrEmpty(dto.FirstName)) user.FirstName = dto.FirstName;
        if (!string.IsNullOrEmpty(dto.LastName))  user.LastName  = dto.LastName;
        if (!string.IsNullOrEmpty(dto.Email))
        {
            user.Email = dto.Email;
            user.UserName = dto.Email;  // keep them in sync
        }

        return await userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> DeactivateUserAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        // Don't allow deactivating the last admin
        if (await userManager.IsInRoleAsync(user, "Admin"))
        {
            var admins = await userManager.GetUsersInRoleAsync("Admin");
            var activeAdmins = admins.Count(a => a.IsActive);
            if (activeAdmins <= 1)
                return IdentityResult.Failed(new IdentityError 
                { 
                    Description = "Cannot deactivate the last active administrator" 
                });
        }

        user.Deactivate();
        return await userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> ActivateUserAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        user.Activate();
        return await userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> AssignRoleAsync(string userId, string role)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        if (!await roleManager.RoleExistsAsync(role))
            return IdentityResult.Failed(new IdentityError { Description = $"Role '{role}' doesn't exist" });
        return await userManager.AddToRoleAsync(user, role);
    }

    public async Task<IdentityResult> RemoveRoleAsync(string userId, string role)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        return await userManager.RemoveFromRoleAsync(user, role);
    }

    public async Task<List<string>> GetUserRolesAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return new List<string>();
        var roles = await userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    private static AdminUserDto MapToDto(AppUser u, List<string> roles) => new(
        u.Id, u.Email ?? "", u.FirstName, u.LastName, u.Department,
        u.Status.ToString(), u.IsActive,
        u.CreatedAt, u.DeactivatedAt, roles);
}
