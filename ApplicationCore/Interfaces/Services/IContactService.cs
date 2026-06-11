using ApplicationCore.Models;

namespace ApplicationCore.Interfaces.Services;

public interface IContactService
{
    public ContactResultDto Create(ContactCreateDto contactDto);
}
