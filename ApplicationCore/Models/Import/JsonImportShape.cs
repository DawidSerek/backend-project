namespace ApplicationCore.Models.Import;

public class JsonImportShape
{
    public List<PersonJson>? People { get; set; }
    public List<CompanyJson>? Companies { get; set; }
    public List<OrganizationJson>? Organizations { get; set; }
}

public class PersonJson
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Pesel { get; set; }
}

public class CompanyJson
{
    public string? Name { get; set; }
    public string? Nip { get; set; }
    public string? Email { get; set; }
    public string? Industry { get; set; }
    public string? Website { get; set; }
}

public class OrganizationJson
{
    public string? Name { get; set; }
}
