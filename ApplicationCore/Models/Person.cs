using ApplicationCore.ValueObjects.Pesel;
using ApplicationCore.ValueObjects;
namespace ApplicationCore.Models;

public class Person : Contact
{
    public required Pesel PESEL { get; set; }
    public Organization? Organization { get; set; }
    public Company? Employer { get; set; }
}

public record CreatePersonDto
{
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Pesel { get; set; }
    public Guid? OrganizationId { get; set; }
}

public record ResultPersonDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public DateTime CreatedDate { get; init; }
    public Pesel PESEL { get; init; } = null!;
    public Guid? OrganizationId { get; init; }
}
