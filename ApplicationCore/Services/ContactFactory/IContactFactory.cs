using ApplicationCore.Models;
using ApplicationCore.Models.Create;
using ApplicationCore.Services.ContactFactory;

namespace ApplicationCore.Services.ContactFactory;

public interface IContactFactory
{
    Person CreatePerson(PersonCreateDto dto, Guid userId);
    Company CreateCompany(CompanyCreateDto dto, Guid userId);
    Organization CreateOrganization(OrganizationCreateDto dto, Guid userId);
}
