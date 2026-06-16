using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Models;
using Infrastructure.EntityFramework.Context;

namespace Infrastructure.EntityFramework.Repository;

public class ContactRepository(AppDbContext context) : GenericRepository<Contact>(context), IContactRepository
{
    public bool ExistsByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        return DbSet.Any(c => c.Email != null && c.Email.ToLower() == email.ToLower());
    }

    public IEnumerable<Contact> FindByEmailDomain(string domain)
    {
        return [.. DbSet
            .Where(x => x.Email != null)
            .Where(x => x.Email!.ToLower().EndsWith('@' + domain.ToLower()))];
    }

    public IEnumerable<Person> FindByOrganizationId(Guid organizationId)
    {
        return [.. DbSet
            .OfType<Person>()
            .Where(x => x.Organization != null)
            .Where(x => x.Organization!.Id == organizationId)];
    }

    public IEnumerable<Contact> Search(ContactSearchCriteriaDto dto)
    {
        IQueryable<Contact> query = DbSet;

        if (!string.IsNullOrEmpty(dto.NameContains))
        {
            query = query.Where(x => x.Name.ToLower().Contains(dto.NameContains.ToLower()));
        }

        if (!string.IsNullOrEmpty(dto.EmailDomain))
        {
            query = query
                .Where(x => x.Email != null)
                .Where(x => x.Email!.ToLower().Contains('@' + dto.EmailDomain));
        }

        if (dto.OrganizationId.HasValue)
        {
            query = query
                .OfType<Person>()
                .Where(x => x.Organization != null)
                .Where(x => x.Organization!.Id == dto.OrganizationId);
        }

        return [.. query
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
        ];
    }
}
