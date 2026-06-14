using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Services.Ef.ContactService;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController(IContactService contactService) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "API is working" });
    }

    [HttpPost]
    public IActionResult Post(CreateContactDto contact)
    {
        var result = contactService.Create(contact);

        return CreatedAtAction(nameof(Get), new { id = result.Id.ToString() }, result);
    }
}
