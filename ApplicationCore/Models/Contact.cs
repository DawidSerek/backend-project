using ApplicationCore.Primitives;
using ApplicationCore.ValueObjects;

namespace ApplicationCore.Models;

public abstract class Contact : EntityBase
{
    public Guid CreatedById { get; set; }
    public required string Name { get; set; }
    public EmailAddress? Email { get; set; }
    public PhoneNumber? PhoneNumber { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

public record CreateContactDto
{
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}

public record ResultContactDto
{
    public Guid Id { get; init; }
    public DateTime CreatedDate { get; init; }
}