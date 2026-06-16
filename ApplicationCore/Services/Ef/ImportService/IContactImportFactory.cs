using ApplicationCore.Models;
using ApplicationCore.Models.Import;

namespace ApplicationCore.Services.Ef.ImportService;

public interface IContactImportFactory
{
    Person CreatePerson(string name, string? email, string? phone, string pesel, Guid userId);
    Company CreateCompany(string name, string? email, string nip, Guid userId);
    Organization CreateOrganization(string name, Guid userId);

    Contact CreateFromImportRow(ImportRow row, Guid userId);
}
