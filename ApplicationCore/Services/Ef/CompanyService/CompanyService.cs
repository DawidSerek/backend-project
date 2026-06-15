using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using AutoMapper;

namespace ApplicationCore.Services.Ef.CompanyService;

public class CompanyService(IUnitOfWork unitOfWork, IMapper mapper) : ICompanyService
{
    public ResultCompanyDto Post(CreateCompanyDto companyDto)
    {
        var company = mapper.Map<Company>(companyDto);

        unitOfWork.Companies.Add(company);

        return mapper.Map<ResultCompanyDto>(company);
    }

    public IEnumerable<ResultPersonDto> GetEmployees(Guid companyId)
    {
        return unitOfWork.Companies.GetEmployees(companyId)
            .Select(p => mapper.Map<ResultPersonDto>(p));
    }

    public IEnumerable<ResultPersonDto> GetEmployeesSorted(
        Guid companyId,
        EmployeeSortField sortBy = EmployeeSortField.Name,
        bool descending = false)
    {
        var employees = unitOfWork.Companies.GetEmployeesSorted(companyId, sortBy, descending);
        return employees.Select(p => mapper.Map<ResultPersonDto>(p));
    }
}
