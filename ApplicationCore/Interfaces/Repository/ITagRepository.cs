using ApplicationCore.Models;

namespace ApplicationCore.Interfaces.Repository;

public interface ITagRepository : IGenericRepository<Tag>
{
    Task<Tag?> GetByNameAsync(string name);
    Task<bool> ExistsAsync(string name);
}