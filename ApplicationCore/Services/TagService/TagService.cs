
using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;

namespace ApplicationCore.Services.TagService;

public class TagService(
    ITagRepository tags,
    IUnitOfWork uow) : ITagService
{
    public async Task<Tag> CreateAsync(string name, string? color = null)
    {
        var existing = await tags.GetByNameAsync(name);
        if (existing is not null)
            return existing;  // already exists, return it (idempotent)

        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = name,
            Color = color
        };
        tags.Add(tag);
        await uow.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag> GetOrCreateAsync(string name) => await CreateAsync(name);

    public async Task AddTagToContactAsync(Guid contactId, string tagName)
    {
        var contact = uow.Contacts.FindById(contactId)
            ?? throw new KeyNotFoundException($"Contact {contactId} not found");
        var tag = await GetOrCreateAsync(tagName);

        if (!contact.Tags.Any(t => t.Id == tag.Id))
        {
            contact.Tags.Add(tag);
            await uow.SaveChangesAsync();
        }
    }

    public async Task RemoveTagFromContactAsync(Guid contactId, string tagName)
    {
        var contact = uow.Contacts.FindById(contactId)
            ?? throw new KeyNotFoundException($"Contact {contactId} not found");
        var tagToRemove = contact.Tags.FirstOrDefault(t => t.Name == tagName);

        if (tagToRemove is not null)
        {
            contact.Tags.Remove(tagToRemove);
            await uow.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Contact>> FindByTagAsync(string tagName)
    {
        return [.. uow.Contacts.GetAll()
            .Where(c => c.Tags.Any(t => t.Name == tagName))];
    }
}