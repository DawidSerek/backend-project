using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using AutoMapper;

namespace ApplicationCore.Services.Ef.OrganizationService;

public class OrganizationService(IUnitOfWork unitOfWork, IMapper mapper) : IOrganizationService
{
    public ResultContactDto Post(CreateContactDto orgDto)
    {
        var org = mapper.Map<Organization>(orgDto);

        unitOfWork.Organizations.Add(org);

        return mapper.Map<ResultContactDto>(org);
    }
}
