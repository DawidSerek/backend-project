using ApplicationCore.Models.Import;
namespace ApplicationCore.Services.Ef.ImportService;

using ApplicationCore.Interfaces.Import;
using ApplicationCore.Models.Import;


public class ContactImportService(IEnumerable<IContactFileParser> parsers)
    : IContactImportService
{
    public Task<ImportReport> ImportAsync(Stream stream, string ext, Guid userId)
    {
        // Find the right parser
        var parser = parsers.FirstOrDefault(p => p.CanParse(ext))
            ?? throw new NotSupportedException($"No parser for .{ext} files");

        return parser.ParseAsync(stream, userId);
    }
}