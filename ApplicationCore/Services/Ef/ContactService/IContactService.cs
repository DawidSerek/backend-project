using ApplicationCore.Models;
using ApplicationCore.Models.Create;

namespace ApplicationCore.Services.Ef.ContactService;

public interface IContactService
{
    Task<Contact> CreateAsync(ContactCreateBase dto, Guid userId);
}
