using ApplicationCore.Models.Import;

namespace ApplicationCore.Interfaces.Import;

public interface IContactFileParser
{
    bool CanParse(string fileExtension);  // "csv" or "json"
    Task<ImportReport> ParseAsync(Stream stream, Guid currentUserId);
}