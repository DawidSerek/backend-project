using ApplicationCore.ValueObjects.Nip;

namespace ApplicationCore.Models;

public class Company : Contact
{
    public required Nip Nip { get; set; }
    public string? Regon { get; set; }
    public string? Industry { get; set; }
    public string? Website { get; set; }

    public List<Person> Employees { get; set; } = [];
}

public record CreateCompanyDto
{
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Nip { get; set; }
    public string? Regon { get; set; }
    public string? Industry { get; set; }
    public string? Website { get; set; }
}

public record ResultCompanyDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public DateTime CreatedDate { get; init; }
    public Nip Nip { get; init; } = null!;
    public string? Regon { get; init; }
    public string? Industry { get; init; }
    public string? Website { get; init; }
}
