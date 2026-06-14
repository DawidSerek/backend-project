using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using ApplicationCore.Services.Ef.PersonService;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonController(IUnitOfWork unitOfWork, IPersonService personService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        var persons = personService.GetAll();
        return Ok(persons);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var person = personService.GetById(id);
        return person is null ? NotFound() : Ok(person);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreatePersonDto dto)
    {
        try
        {
            var result = personService.Post(dto);
            await unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        unitOfWork.Persons.RemoveById(id);
        await unitOfWork.SaveChangesAsync();
        return NoContent();
    }
}
