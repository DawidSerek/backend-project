using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using ApplicationCore.Models.Create;
using ApplicationCore.Services.Ef.ContactFactory;

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
}
