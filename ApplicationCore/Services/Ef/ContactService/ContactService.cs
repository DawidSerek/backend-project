using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Interfaces.Validation;
using ApplicationCore.Models;
using ApplicationCore.Models.Create;
using ApplicationCore.Services.ContactFactory;
using ApplicationCore.Services.DeduplicationStrategy;
using ApplicationCore.ValueObjects;

namespace ApplicationCore.Services.Ef.ContactService;

public class ContactService(
    IContactFactory factory,
    IUnitOfWork uow,
    IKrsValidator krs,
    IWebsiteValidator website,
    IDeduplicationStrategyService dedup
) : IContactService
{
    public async Task<Contact> CreateAsync(ContactCreateBase dto, Guid userId)
    {
        if (dto is OrganizationCreateDto orgDto)
        {
            if (orgDto.Krs is not null && !await krs.ValidateKrsAsync(orgDto.Krs))
                throw new ArgumentException($"KRS '{orgDto.Krs}' is not valid");
            if (orgDto.Website is not null && !await website.ValidateAsync(orgDto.Website))
                throw new ArgumentException($"Website '{orgDto.Website}' is not reachable");
        }
        Contact contact = dto switch
        {
            PersonCreateDto p => factory.CreatePerson(p, userId),
            CompanyCreateDto c => factory.CreateCompany(c, userId),
            OrganizationCreateDto o => factory.CreateOrganization(o, userId),
            _ => throw new ArgumentException("Unknown contact type")
        };

        var dedupConfig = new DeduplicationConfigDto(
            Threshold: 1.0,
            Properties: new List<string> { "name", "email", "phonenumber" },
            Strategy: DeduplicationStrategyOptions.Exact);
        if (await dedup.IsDuplicateOfExistingAsync(contact, dedupConfig))
            throw new ArgumentException("Contact already exists");

        uow.Contacts.Add(contact);
        await uow.SaveChangesAsync();
        return contact;
    }

    public async Task<Contact?> UpdateAsync(Guid id, ContactCreateBase dto)
    {
        var contact = uow.Contacts.FindById(id);
        if (contact is null) return null;

        switch (contact, dto)
        {
            case (Person person, PersonCreateDto pdto):
                person.Name = pdto.Name;
                person.Email = new EmailAddress(pdto.Email);
                person.PhoneNumber = pdto.Phone is null ? null : new PhoneNumber(pdto.Phone);
                break;
            case (Company company, CompanyCreateDto cdto):
                company.Name = cdto.Name;
                company.Email = new EmailAddress(cdto.Email);
                company.PhoneNumber = cdto.Phone is null ? null : new PhoneNumber(cdto.Phone);
                company.Regon = cdto.Regon;
                company.Industry = cdto.Industry;
                company.Website = cdto.Website;
                break;
            case (Organization org, OrganizationCreateDto odto):
                org.Name = odto.Name;
                org.Email = new EmailAddress(odto.Email);
                org.PhoneNumber = odto.Phone is null ? null : new PhoneNumber(odto.Phone);
                break;
            default:
                throw new ArgumentException("Contact type mismatch");
        }

        await uow.SaveChangesAsync();
        return contact;
    }
}
