using ApplicationCore.Models.Interactions;

namespace ApplicationCore.Interfaces.Repository;

public interface IInteractionRepository
{
    Task<Interaction> AddAsync(Interaction interaction);

    Task<Interaction?> GetByIdAsync(Guid id);

    Task<IEnumerable<Interaction>> GetByContactAsync(
        Guid contactId, int page = 1, int pageSize = 20);

    Task<IEnumerable<Interaction>> GetByContactInRangeAsync(
        Guid contactId, DateTime from, DateTime to);

    Task<IEnumerable<Interaction>> GetByContactAndTypeAsync(
        Guid contactId, InteractionType type);

    Task RemoveAsync(Guid id);
}