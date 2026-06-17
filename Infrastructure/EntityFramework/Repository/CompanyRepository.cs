using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Models;
using ApplicationCore.ValueObjects;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repository;

public class CompanyRepository(AppDbContext context) : GenericRepository<Company>(context), ICompanyRepository
{
    public Company? GetByNip(string nip)
    {
        return DbSet.FirstOrDefault(x => x.Nip.Value == nip);
    }

    public IEnumerable<Person> GetEmployees(Guid companyId)
    {
        var company = DbSet
            .Include(c => c.Employees)
            .FirstOrDefault(c => c.Id == companyId);

        return company?.Employees.ToList() ?? [];
    }

    public IEnumerable<Person> GetEmployeesSorted(Guid companyId, EmployeeSortField sortBy = EmployeeSortField.Name, bool descending = false)
    {
        var employees = context.Persons
            .Include(x => x.Employer)
            .Where(x => x.Employer != null && x.Employer.Id == companyId)
            .ToList();

        Func<Person, IComparable> keySelector = sortBy switch
        {
            EmployeeSortField.Name => p => p.Name,
            EmployeeSortField.Email => p => p.Email != null ? p.Email.Value : string.Empty,
            EmployeeSortField.BirthDate => p => p.PESEL.BirthDate,
            _ => p => p.Name
        };

        return descending
            ? [.. employees.OrderByDescending(keySelector)]
            : [.. employees.OrderBy(keySelector)];
    }

    public IEnumerable<Company> SearchByName(string namePart)
    {
        return DbSet.Where(x => x.Name.Contains(namePart));
    }
}
