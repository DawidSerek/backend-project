using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public enum SystemUserStatus
{
    Active = 0,
    Inactive = 1,
    Locked = 2
}

public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public SystemUserStatus Status { get; set; } = SystemUserStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeactivatedAt { get; set; }

    public bool IsActive => Status == SystemUserStatus.Active;

    public void Deactivate()
    {
        if (Status == SystemUserStatus.Active)
        {
            Status = SystemUserStatus.Inactive;
            DeactivatedAt = DateTime.UtcNow;
        }
    }

    public void Activate()
    {
        if (Status != SystemUserStatus.Active)
        {
            Status = SystemUserStatus.Active;
            DeactivatedAt = null;
        }
    }
}
