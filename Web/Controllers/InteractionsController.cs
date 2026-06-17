using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models.Interactions;
using ApplicationCore.Services.Ef.InteractionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/contacts/{contactId:guid}/interactions")]
[Authorize]
public class InteractionsController(IInteractionService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add(Guid contactId, [FromBody] CreateInteractionDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await service.AddAsync(contactId, dto, userId);
            return CreatedAtAction(nameof(GetHistory), new { contactId }, result);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
    }

    [HttpGet]
    public async Task<IActionResult> GetHistory(
        Guid contactId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] InteractionType? type,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await service.GetHistoryAsync(contactId, from, to, type, page, pageSize);
        return Ok(result);
    }

    [HttpDelete("/api/interactions/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");
            await service.DeleteAsync(id, userId, isAdmin);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim is null ? Guid.Empty : Guid.Parse(claim.Value);
    }
}