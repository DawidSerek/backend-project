using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using ApplicationCore.Models.Create;
using ApplicationCore.ValueObjects;
using ApplicationCore.Services.ContactFactory;

namespace ApplicationCore.Services.Ef.ContactService;

public class ContactService(IContactFactory factory, IUnitOfWork uow) : IContactService
{
    public async Task<Contact> CreateAsync(ContactCreateBase dto, Guid userId)
    {
        Contact contact = dto switch
        {
            PersonCreateDto p => factory.CreatePerson(p, userId),
            CompanyCreateDto c => factory.CreateCompany(c, userId),
            OrganizationCreateDto o => factory.CreateOrganization(o, userId),
            _ => throw new ArgumentException("Unknown contact type")
        };

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
