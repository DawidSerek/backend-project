using ApplicationCore.Models;

namespace ApplicationCore.Services.Ef.OrganizationService;

public interface IOrganizationService
{
    public ResultContactDto Post(CreateContactDto contactDto);
}