using ApplicationCore.Models;

namespace ApplicationCore.Interfaces.Repository;

public interface IPersonRepository : IGenericRepository<Person>
{
    IEnumerable<Person> GetAllWithOrganization();
}
