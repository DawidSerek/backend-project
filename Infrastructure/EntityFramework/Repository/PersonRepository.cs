using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Models;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repository;

public class PersonRepository(AppDbContext context) : GenericRepository<Person>(context), IPersonRepository
{
    public IEnumerable<Person> GetAllWithOrganization()
    {
        return [.. DbSet.Include(p => p.Organization)];
    }
}
