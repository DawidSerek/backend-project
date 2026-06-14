using ApplicationCore.Models;
using ApplicationCore.Interfaces.UnitOfWork;
using AutoMapper;

namespace ApplicationCore.Services.Ef.ContactService;

public class ContactService(IUnitOfWork unitOfWork, IMapper mapper) : IContactService
{
    public ResultContactDto Create(CreateContactDto contactDto)
    {
        var contact = mapper.Map<Contact>(contactDto);

        var result = unitOfWork.Contacts.Add(contact);

        return mapper.Map<ResultContactDto>(result);
    }
}
