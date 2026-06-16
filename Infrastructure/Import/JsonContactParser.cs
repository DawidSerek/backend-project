using System.Text.Json;
using ApplicationCore.Interfaces.Import;
using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using ApplicationCore.Models.Import;
using ApplicationCore.Services.Ef.ImportService;

namespace Infrastructure.Import;

public class JsonContactParser(IUnitOfWork uow, IContactImportFactory factory) : IContactFileParser
{
    public bool CanParse(string ext) => ext.ToLower() is "json";

    public async Task<ImportReport> ParseAsync(Stream stream, Guid currentUserId)
    {
        var report = new ImportReport([], []);
        var seen = new HashSet<string>();

        var data = await JsonSerializer.DeserializeAsync<JsonImportShape>(stream, JsonOptionsFactory.Default);
        if (data is null)
        {
            report.Errors.Add(new FailedImport([], ["Invalid JSON structure"]));
            return report;
        }

        ProcessPeople(data.People, seen, report, currentUserId);
        ProcessCompanies(data.Companies, seen, report, currentUserId);
        ProcessOrganizations(data.Organizations, seen, report, currentUserId);

        await uow.SaveChangesAsync();
        return report;
    }

    private void ProcessPeople(List<PersonJson>? people, HashSet<string> seen, ImportReport report, Guid userId)
    {
        if (people is null) return;
        foreach (var p in people)
        {
            var key = $"Person|{p.Email}";
            if (!seen.Add(key))
            {
                report.Errors.Add(new FailedImport(
                    new Dictionary<string, string> { ["email"] = p.Email ?? "" },
                    ["Duplicate within file"]));
                continue;
            }

            try
            {
                var person = factory.CreatePerson(p.Name ?? "", p.Email, p.Phone, p.Pesel ?? "", userId);
                if (!string.IsNullOrEmpty(p.Email) && uow.Contacts.ExistsByEmail(p.Email))
                {
                    report.Errors.Add(new FailedImport(
                        new Dictionary<string, string> { ["email"] = p.Email },
                        ["Contact with this email already exists"]));
                    continue;
                }

                uow.Contacts.Add(person);
                report.Imported.Add(new ImportedSummary(person.Name, "Person", person.Id));
            }
            catch (Exception ex)
            {
                report.Errors.Add(new FailedImport(
                    new Dictionary<string, string> { ["pesel"] = p.Pesel ?? "" },
                    [ex.Message]));
            }
        }
    }

    private void ProcessCompanies(List<CompanyJson>? companies, HashSet<string> seen, ImportReport report, Guid userId)
    {
        if (companies is null) return;
        foreach (var c in companies)
        {
            var key = $"Company|{c.Email}";
            if (!seen.Add(key))
            {
                report.Errors.Add(new FailedImport(
                    new Dictionary<string, string> { ["email"] = c.Email ?? "" },
                    ["Duplicate within file"]));
                continue;
            }

            try
            {
                var company = factory.CreateCompany(c.Name ?? "", c.Email, c.Nip ?? "", userId);
                if (!string.IsNullOrEmpty(c.Email) && uow.Contacts.ExistsByEmail(c.Email))
                {
                    report.Errors.Add(new FailedImport(
                        new Dictionary<string, string> { ["email"] = c.Email },
                        ["Contact with this email already exists"]));
                    continue;
                }

                uow.Contacts.Add(company);
                report.Imported.Add(new ImportedSummary(company.Name, "Company", company.Id));
            }
            catch (Exception ex)
            {
                report.Errors.Add(new FailedImport(
                    new Dictionary<string, string> { ["nip"] = c.Nip ?? "" },
                    [ex.Message]));
            }
        }
    }

    private void ProcessOrganizations(List<OrganizationJson>? orgs, HashSet<string> seen, ImportReport report, Guid userId)
    {
        if (orgs is null) return;
        foreach (var o in orgs)
        {
            var key = $"Organization|{o.Name}";
            if (!seen.Add(key))
            {
                report.Errors.Add(new FailedImport(
                    new Dictionary<string, string> { ["name"] = o.Name ?? "" },
                    ["Duplicate within file"]));
                continue;
            }

            try
            {
                var org = factory.CreateOrganization(o.Name ?? "", userId);
                uow.Contacts.Add(org);
                report.Imported.Add(new ImportedSummary(org.Name, "Organization", org.Id));
            }
            catch (Exception ex)
            {
                report.Errors.Add(new FailedImport(
                    new Dictionary<string, string> { ["name"] = o.Name ?? "" },
                    [ex.Message]));
            }
        }
    }
}
