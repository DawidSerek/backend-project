namespace ApplicationCore.Models.Import;

public class ImportRow
{
    public string ContactType { get; set; } = "";  // "Person", "Company", "Organization"
    public string GroupName  { get; set; } = "";  // the section header
    public Dictionary<string, string> Fields { get; set; } = new();

    public static ImportRow FromCsvRow(string group, string[] headers, string[] fields) => new()
    {
        ContactType = GroupNameMapper.MapToContactType(group),
        GroupName = group,
        Fields = headers.Zip(fields, (h, v) => new { h, v })
                         .ToDictionary(p => p.h, p => p.v.Trim())
    };
}

public record ImportReport(
    List<ImportedSummary> Imported,
    List<FailedImport> Errors
);

public record ImportedSummary(
    string Name,
    string Type,
    Guid Id
);

public record FailedImport(
    Dictionary<string, string> Data,
    List<string> ErrorMessages
);