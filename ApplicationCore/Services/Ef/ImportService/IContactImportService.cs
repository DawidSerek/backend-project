using ApplicationCore.Models.Import;

namespace ApplicationCore.Services.Ef.ImportService;

public interface IContactImportService
{
    Task<ImportReport> ImportAsync(Stream stream, string fileExtension, Guid currentUserId);
}

