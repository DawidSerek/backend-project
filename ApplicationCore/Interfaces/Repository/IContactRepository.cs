using ApplicationCore.Models;
namespace ApplicationCore.Interfaces.Repository;

// Find by email
// Find by organization
// Search
public interface IContactRepository : IGenericRepository<Contact>
{
    IEnumerable<Contact> FindByEmailDomain(string domain);
    IEnumerable<Person> FindByOrganizationId(Guid id);
    IEnumerable<Contact> Search(ContactSearchCriteriaDto dto);
}

public record ContactSearchCriteriaDto(
    string? EmailDomain = null,
    Guid? OrganizationId = null,
    string? NameContains = null,
    int Page = 1,
    int PageSize = 20
);