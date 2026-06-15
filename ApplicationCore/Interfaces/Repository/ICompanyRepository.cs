using ApplicationCore.Models;

namespace ApplicationCore.Interfaces.Repository;

public interface ICompanyRepository : IGenericRepository<Company>
{
    Company? GetByNip(string nip);

    IEnumerable<Company> SearchByName(string namePart);

    IEnumerable<Person> GetEmployees(Guid companyId);

    IEnumerable<Person> GetEmployeesSorted(
        Guid companyId,
        EmployeeSortField sortBy = EmployeeSortField.Name,
        bool descending = false);
}

public enum EmployeeSortField
{
    Name,
    Email,
    BirthDate
}
