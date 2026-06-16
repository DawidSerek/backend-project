using ApplicationCore.Services.Ef.ImportService;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportController(IContactImportService import) : ControllerBase
{
    [HttpPost("contacts")]
    [RequestSizeLimit(100_000_000)]  // 100 MB
    public async Task<IActionResult> ImportContacts(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file uploaded");

        var ext = Path.GetExtension(file.FileName).TrimStart('.');
        if (ext != "csv" && ext != "json")
            return BadRequest("Only .csv and .json files are supported");

        using var stream = file.OpenReadStream();
        var report = await import.ImportAsync(stream, ext, Guid.Empty);  // TODO: current user

        return Ok(report);
    }
}
