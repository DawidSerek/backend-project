using ApplicationCore.Services.DeduplicationStrategy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,SalesManager")]
public class DeduplicationController(IDeduplicationStrategyService service) : ControllerBase
{
    [HttpPost("find")]
    public async Task<IActionResult> Find([FromBody] DeduplicationConfigDto config)
    {
        if (config.Threshold < 0 || config.Threshold > 1)
            return BadRequest("Threshold must be in [0, 1]");

        var report = await service.FindDuplicatesAsync(config);
        return Ok(report);
    }

    [HttpPost("remove")]
    public async Task<IActionResult> Remove([FromBody] DeduplicationConfigDto config)
    {
        if (config.Threshold < 0 || config.Threshold > 1)
            return BadRequest("Threshold must be in [0, 1]");

        var userId = GetCurrentUserId();
        var report = await service.RemoveDuplicatesAsync(config, userId);
        return Ok(report);
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim is null ? Guid.Empty : Guid.Parse(claim.Value);
    }
}

