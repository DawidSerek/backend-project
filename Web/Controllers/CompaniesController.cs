using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using ApplicationCore.Services.Ef.CompanyService;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController(IUnitOfWork unitOfWork, ICompanyService companyService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        var companies = unitOfWork.Companies.GetAll();
        return Ok(companies);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var company = unitOfWork.Companies.FindById(id);
        return company is null ? NotFound() : Ok(company);
    }

    [HttpGet("by-nip/{nip}")]
    public IActionResult GetByNip(string nip)
    {
        var company = unitOfWork.Companies.GetByNip(nip);
        return company is null ? NotFound() : Ok(company);
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string? name, [FromQuery] string? nip)
    {
        if (!string.IsNullOrEmpty(nip))
        {
            var byNip = unitOfWork.Companies.GetByNip(nip);
            return byNip is null ? NotFound() : Ok(byNip);
        }
        if (!string.IsNullOrEmpty(name))
            return Ok(unitOfWork.Companies.SearchByName(name));
        return BadRequest("Provide either ?name=... or ?nip=...");
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateCompanyDto dto)
    {
        try
        {
            var result = companyService.Post(dto);
            await unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var company = unitOfWork.Companies.FindById(id);
        if (company is null) return NotFound();

        unitOfWork.Companies.RemoveById(id);
        await unitOfWork.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id:guid}/employees")]
    public IActionResult GetEmployees(Guid id)
    {
        if (unitOfWork.Companies.FindById(id) is null) return NotFound();
        return Ok(companyService.GetEmployees(id));
    }

    [HttpGet("{id:guid}/employees/sorted")]
    public IActionResult GetEmployeesSorted(
        Guid id,
        [FromQuery] string sortBy = "name",
        [FromQuery] bool desc = false)
    {
        if (unitOfWork.Companies.FindById(id) is null) return NotFound();

        EmployeeSortField field = sortBy.ToLower() switch
        {
            "name" => EmployeeSortField.Name,
            "email" => EmployeeSortField.Email,
            "birthdate" => EmployeeSortField.BirthDate,
            _ => EmployeeSortField.Name
        };

        return Ok(companyService.GetEmployeesSorted(id, field, desc));
    }
}
