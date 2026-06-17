using ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController(IAdminService admin) : ControllerBase
{
    [HttpGet("users")]
    public async Task<IActionResult> GetAll() => Ok(await admin.GetAllUsersAsync());

    [HttpGet("users/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var user = await admin.GetUserAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost("users")]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest req,
        [FromQuery] string password = "Temp123!")
    {
        var dto = new CreateUserDto(req.Email, req.FirstName, req.LastName, req.Roles);
        var result = await admin.CreateUserAsync(dto, password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });
        return Created();
    }

    [HttpPatch("users/{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
    {
        var result = await admin.UpdateUserAsync(id, dto);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });
        return NoContent();
    }

    [HttpPost("users/{id}/deactivate")]
    public async Task<IActionResult> Deactivate(string id)
    {
        var result = await admin.DeactivateUserAsync(id);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });
        return NoContent();
    }

    [HttpPost("users/{id}/activate")]
    public async Task<IActionResult> Activate(string id)
    {
        var result = await admin.ActivateUserAsync(id);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });
        return NoContent();
    }

    [HttpPost("users/{id}/roles")]
    public async Task<IActionResult> AddRole(string id, [FromBody] string role)
    {
        var result = await admin.AssignRoleAsync(id, role);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });
        return NoContent();
    }

    [HttpDelete("users/{id}/roles/{role}")]
    public async Task<IActionResult> RemoveRole(string id, string role)
    {
        var result = await admin.RemoveRoleAsync(id, role);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });
        return NoContent();
    }
}

public record CreateUserRequest(string Email, string FirstName, string LastName, string[] Roles);
