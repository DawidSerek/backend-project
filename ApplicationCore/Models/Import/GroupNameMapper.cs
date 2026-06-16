namespace ApplicationCore.Models.Import;

public static class GroupNameMapper
{
    public const string Unknown = "Unknown";

    public static string MapToContactType(string group) => group.ToLower() switch
    {
        "people" or "persons" or "person" => "Person",
        "companies" or "company" => "Company",
        "organizations" or "organisation" or "organisationen" or "organization" => "Organization",
        _ => Unknown
    };
}
