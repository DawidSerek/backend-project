using ApplicationCore.Models;
using ApplicationCore.Models.Create;

namespace ApplicationCore.Services.Ef.ContactFactory;

public interface IContactFactory
{
    Person CreatePerson(PersonCreateDto dto, Guid userId);
    Company CreateCompany(CompanyCreateDto dto, Guid userId);
    Organization CreateOrganization(OrganizationCreateDto dto, Guid userId);
}
