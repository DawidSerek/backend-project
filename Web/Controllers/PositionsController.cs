using ApplicationCore.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController(IPositionRepository positions) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var all = await positions.GetAllAsync();
        return Ok(all.Select(p => new { p.Id, p.Name, p.Description }));
    }
}