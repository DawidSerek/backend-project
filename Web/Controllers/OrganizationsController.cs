using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using ApplicationCore.Services.Ef.OrganizationService;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController(
    IUnitOfWork unitOfWork,
    IOrganizationService organizationService
) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        var org = unitOfWork.Organizations.GetAll();
        return Ok(org);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var org = unitOfWork.Organizations.FindById(id);
        return org is null ? NotFound() : Ok(org);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateContactDto dto)
    {
        var result = organizationService.Post(dto);
        await unitOfWork.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        unitOfWork.Organizations.RemoveById(id);
        await unitOfWork.SaveChangesAsync();
        return NoContent();
    }
}
