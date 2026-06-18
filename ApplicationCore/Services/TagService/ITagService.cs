using ApplicationCore.Models;

namespace ApplicationCore.Services.TagService;

public interface ITagService
{
    Task<Tag> CreateAsync(string name, string? color = null);
    Task<Tag> GetOrCreateAsync(string name);  // for use in imports
    Task AddTagToContactAsync(Guid contactId, string tagName);
    Task RemoveTagFromContactAsync(Guid contactId, string tagName);
    Task<IEnumerable<Contact>> FindByTagAsync(string tagName);
}
