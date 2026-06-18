namespace Web.Controllers;

using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models.Create;
using ApplicationCore.Services.Ef.ContactService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ContactsController(IContactService service, IUnitOfWork uow) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var c = uow.Contacts.FindById(id);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPost]
    // [Authorize]
    public async Task<IActionResult> Create([FromBody] ContactCreateBase dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await service.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)  { return BadRequest(new { error = ex.Message }); }
        catch (KeyNotFoundException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("{id:guid}")]
    // [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var c = uow.Contacts.FindById(id);
        if (c is null) return NotFound();

        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");

        if (c.CreatedById != userId && !isAdmin)
            return Forbid();

        uow.Contacts.RemoveById(id);
        await uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    // [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] ContactCreateBase dto)
    {
        var c = uow.Contacts.FindById(id);
        if (c is null) return NotFound();

        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");

        if (c.CreatedById != userId && !isAdmin)
            return Forbid();

        try
        {
            var result = await service.UpdateAsync(id, dto);
            return Ok(result);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim is null ? Guid.Empty : Guid.Parse(claim.Value);
    }
}
