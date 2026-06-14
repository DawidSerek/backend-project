using ApplicationCore.Models;

namespace ApplicationCore.Services.Ef.ContactService;

public interface IContactService
{
    public ResultContactDto Create(CreateContactDto contactDto);
}
