using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using AutoMapper;

namespace ApplicationCore.Services.Ef.PersonService;

public class PersonService(IUnitOfWork unitOfWork, IMapper mapper) : IPersonService
{
    public ResultContactDto Post(CreatePersonDto personDto)
    {
        var person = mapper.Map<Person>(personDto);

        if (personDto.OrganizationId is { } organizationId)
        {
            person.Organization = unitOfWork.Organizations.FindById(organizationId)
                ?? throw new KeyNotFoundException($"Organization {organizationId} not found");
        }

        unitOfWork.Persons.Add(person);

        return mapper.Map<ResultContactDto>(person);
    }

    public IEnumerable<ResultPersonDto> GetAll()
    {
        return unitOfWork.Persons.GetAllWithOrganization().Select(p => mapper.Map<ResultPersonDto>(p));
    }

    public ResultPersonDto? GetById(Guid id)
    {
        var person = unitOfWork.Persons.GetAllWithOrganization().FirstOrDefault(p => p.Id == id);
        return person is null ? null : mapper.Map<ResultPersonDto>(person);
    }
}
