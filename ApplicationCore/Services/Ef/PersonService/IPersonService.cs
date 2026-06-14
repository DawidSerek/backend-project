using ApplicationCore.Models;

namespace ApplicationCore.Services.Ef.PersonService;

public interface IPersonService
{
    public ResultContactDto Post(CreatePersonDto personDto);
    public IEnumerable<ResultPersonDto> GetAll();
    public ResultPersonDto? GetById(Guid id);
}
