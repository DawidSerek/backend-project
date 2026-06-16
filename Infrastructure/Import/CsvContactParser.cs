using ApplicationCore.Interfaces.Import;
using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models.Import;
using ApplicationCore.Services.Ef.ImportService;

namespace Infrastructure.Import;

public class CsvContactParser(IUnitOfWork unitOfWork, IContactImportFactory factory) : IContactFileParser
{
    public bool CanParse(string ext) => ext.ToLower() is "csv" or "tsv";

    public async Task<ImportReport> ParseAsync(Stream stream, Guid currentUserId)
    {
        var report = new ImportReport([], []);
        var seen = new HashSet<string>();

        using var csv = CsvReaderFactory.CreateDefault(stream);

        string? currentGroup = null;
        string[]? currentHeaders = null;
        int lineNum = 0;

        while (await csv.ReadAsync())
        {
            lineNum++;
            var fields = csv.Parser.Record ?? [];
            if (fields.Length == 0 || fields.All(string.IsNullOrWhiteSpace)) continue;

            if (currentGroup is null) { currentGroup = fields[0].Trim(); continue; }
            if (currentHeaders is null) { currentHeaders = [.. fields.Select(f => f.Trim())]; continue; }

            TryImportRow(lineNum, currentGroup, currentHeaders, fields, seen, report, currentUserId);
        }

        await unitOfWork.SaveChangesAsync();
        return report;
    }

    private void TryImportRow(int lineNum, string group, string[] headers, string[] fields,
        HashSet<string> seen, ImportReport report, Guid userId)
    {
        var row = ImportRow.FromCsvRow(group, headers, fields);

        if (row.ContactType == GroupNameMapper.Unknown)
        {
            report.Errors.Add(new FailedImport(row.Fields, [$"Unknown group '{group}'"]));
            return;
        }

        var email = row.Fields.GetValueOrDefault("Email", "");
        var dedupKey = $"{row.ContactType}|{email}";
        if (!seen.Add(dedupKey))
        {
            report.Errors.Add(new FailedImport(row.Fields, ["Duplicate within file"]));
            return;
        }

        try
        {
            var contact = factory.CreateFromImportRow(row, userId);

            if (unitOfWork.Contacts.ExistsByEmail(email))
            {
                report.Errors.Add(new FailedImport(row.Fields, ["Contact with this email already exists"]));
                return;
            }

            unitOfWork.Contacts.Add(contact);
            report.Imported.Add(new ImportedSummary(contact.Name, contact.GetType().Name, contact.Id));
        }
        catch (Exception ex)
        {
            report.Errors.Add(new FailedImport(
                new Dictionary<string, string> { ["line"] = lineNum.ToString() },
                [ex.Message]
            ));
        }
    }
}
