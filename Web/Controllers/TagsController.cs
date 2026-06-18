using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Models;
using ApplicationCore.Services.TagService;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController(ITagService tags, ITagRepository tagRepo) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() =>
        Ok(tagRepo.GetAll().Select(t => new { t.Id, t.Name, t.Color }));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest req)
    {
        var tag = await tags.CreateAsync(req.Name, req.Color);
        return Created($"/api/tags/{tag.Id}", tag);
    }

    [HttpPost("contacts/{contactId:guid}/{tagName}")]
    public async Task<IActionResult> AddToContact(Guid contactId, string tagName)
    {
        await tags.AddTagToContactAsync(contactId, tagName);
        return NoContent();
    }

    [HttpDelete("contacts/{contactId:guid}/{tagName}")]
    public async Task<IActionResult> RemoveFromContact(Guid contactId, string tagName)
    {
        await tags.RemoveTagFromContactAsync(contactId, tagName);
        return NoContent();
    }

    [HttpGet("contacts/by-tag/{tagName}")]
    public async Task<IActionResult> FindByTag(string tagName) =>
        Ok(await tags.FindByTagAsync(tagName));
}

public record CreateTagRequest(string Name, string? Color);
