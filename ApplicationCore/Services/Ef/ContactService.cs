using ApplicationCore.Models;
using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Interfaces.Services;
using AutoMapper;

namespace ApplicationCore.Services.Ef;

public class ContactService(IUnitOfWork unitOfWork, IMapper mapper) : IContactService
{
    public ContactResultDto Create(ContactCreateDto contactDto)
    {
        var contact = mapper.Map<Contact>(contactDto);

        var result = unitOfWork.Contacts.Add(contact);

        return mapper.Map<ContactResultDto>(result);
    }
}
