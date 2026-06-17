using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Models.Interactions;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repository;

public class InteractionRepository(AppDbContext context) : IInteractionRepository
{
    private AppDbContext Ctx => (AppDbContext)context;
    private DbSet<Interaction> Set => Ctx.Set<Interaction>();
    private IQueryable<Interaction> InteractionsWithContact => Set.Include(x => x.Contact);

    public async Task<Interaction> AddAsync(Interaction interaction)
    {
        var entry = await Set.AddAsync(interaction);
        return entry.Entity;
    }

    public Task<Interaction?> GetByIdAsync(Guid id) =>
        Set.FirstOrDefaultAsync(i => i.Id == id);

    public async Task<IEnumerable<Interaction>> GetByContactAsync(
        Guid contactId, int page = 1, int pageSize = 20)
    {
        return await InteractionsWithContact
            .Where(i => i.Contact.Id == contactId)
            .OrderByDescending(i => i.Date)  // newest first
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Interaction>> GetByContactInRangeAsync(
        Guid contactId, DateTime from, DateTime to)
    {
        return await InteractionsWithContact
            .Where(i => i.Contact.Id == contactId && i.Date >= from && i.Date <= to)
            .OrderByDescending(i => i.Date)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Interaction>> GetByContactAndTypeAsync(
        Guid contactId, InteractionType type)
    {
        return await InteractionsWithContact
            .Where(i => i.Contact.Id == contactId && i.Type == type)
            .OrderByDescending(i => i.Date)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task RemoveAsync(Guid id)
    {
        var entity = await Set.FindAsync(id);
        if (entity is not null)
            Set.Remove(entity);
    }
}