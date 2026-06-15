using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Models;

namespace ApplicationCore.Services.Ef.CompanyService;

public interface ICompanyService
{
    public ResultCompanyDto Post(CreateCompanyDto companyDto);
    public IEnumerable<ResultPersonDto> GetEmployees(Guid companyId);
    public IEnumerable<ResultPersonDto> GetEmployeesSorted(
        Guid companyId,
        EmployeeSortField sortBy = EmployeeSortField.Name,
        bool descending = false);
}
