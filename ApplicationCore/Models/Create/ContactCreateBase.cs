using System.Text.Json.Serialization;

namespace ApplicationCore.Models.Create;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "contactType")]
[JsonDerivedType(typeof(PersonCreateDto), "Person")]
[JsonDerivedType(typeof(CompanyCreateDto), "Company")]
[JsonDerivedType(typeof(OrganizationCreateDto), "Organization")]
public abstract class ContactCreateBase;

public class PersonCreateDto : ContactCreateBase
{
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public required string Pesel { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? EmployerId { get; set; }
}

public class CompanyCreateDto : ContactCreateBase
{
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public required string Nip { get; set; }
    public string? Regon { get; set; }
    public string? Industry { get; set; }
    public string? Website { get; set; }
}

public class OrganizationCreateDto : ContactCreateBase
{
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}