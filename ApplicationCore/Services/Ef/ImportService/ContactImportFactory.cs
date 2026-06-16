using ApplicationCore.Models;
using ApplicationCore.Models.Import;
using ApplicationCore.ValueObjects.Nip;
using ApplicationCore.ValueObjects.Pesel;
using ApplicationCore.ValueObjects;

namespace ApplicationCore.Services.Ef.ImportService;

public class ContactImportFactory : IContactImportFactory
{
    public Person CreatePerson(string name, string? email, string? phone, string pesel, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required for Person");
        if (string.IsNullOrWhiteSpace(pesel))
            throw new ArgumentException("Pesel is required for Person");

        return new Person
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            PhoneNumber = phone,
            PESEL = new Pesel(pesel),
            CreatedById = userId
        };
    }

    public Company CreateCompany(string name, string? email, string nip, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required for Company");
        if (string.IsNullOrWhiteSpace(nip))
            throw new ArgumentException("Nip is required for Company");

        return new Company
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Nip = new Nip(nip),
            CreatedById = userId
        };
    }

    public Organization CreateOrganization(string name, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required for Organization");

        return new Organization
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedById = userId
        };
    }

    public Contact CreateFromImportRow(ImportRow row, Guid userId) => row.ContactType switch
    {
        "Person" => CreatePerson(
            row.Fields.GetValueOrDefault("Name", ""),
            row.Fields.GetValueOrDefault("Email"),
            row.Fields.GetValueOrDefault("Phone"),
            row.Fields.GetValueOrDefault("Pesel", ""),
            userId),
        "Company" => CreateCompany(
            row.Fields.GetValueOrDefault("Name", ""),
            row.Fields.GetValueOrDefault("Email"),
            row.Fields.GetValueOrDefault("Nip", ""),
            userId),
        "Organization" => CreateOrganization(
            row.Fields.GetValueOrDefault("Name", ""),
            userId),
        _ => throw new ArgumentException($"Unknown contact type: {row.ContactType}")
    };
}
