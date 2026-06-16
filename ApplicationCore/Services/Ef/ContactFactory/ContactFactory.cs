using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using ApplicationCore.Models.Create;
using ApplicationCore.ValueObjects;
using ApplicationCore.ValueObjects.Nip;
using ApplicationCore.ValueObjects.Pesel;

namespace ApplicationCore.Services.Ef.ContactFactory;

public class ContactFactory(IUnitOfWork uow) : IContactFactory
{
    public Person CreatePerson(PersonCreateDto dto, Guid userId)
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            PhoneNumber = dto.Phone is null ? null : new PhoneNumber(dto.Phone),
            PESEL = new Pesel(dto.Pesel),
            CreatedById = userId
        };

        if (dto.OrganizationId is { } organizationId)
        {
            person.Organization = uow.Organizations.FindById(organizationId)
                ?? throw new KeyNotFoundException($"Organization {organizationId} not found");
        }
        if (dto.EmployerId is { } employerId)
        {
            person.Employer = uow.Companies.FindById(employerId)
                ?? throw new KeyNotFoundException($"Company {employerId} not found");
        }

        return person;
    }

    public Company CreateCompany(CompanyCreateDto dto, Guid userId) => new()
    {
        Id = Guid.NewGuid(),
        Name = dto.Name,
        Email = dto.Email,
        PhoneNumber = dto.Phone is null ? null : new PhoneNumber(dto.Phone),
        Nip = new Nip(dto.Nip),
        Regon = dto.Regon,
        Industry = dto.Industry,
        Website = dto.Website,
        CreatedById = userId
    };

    public Organization CreateOrganization(OrganizationCreateDto dto, Guid userId) => new()
    {
        Id = Guid.NewGuid(),
        Name = dto.Name,
        Email = dto.Email,
        PhoneNumber = dto.Phone is null ? null : new PhoneNumber(dto.Phone),
        CreatedById = userId
    };
}
