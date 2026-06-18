using ApplicationCore.Models;

namespace ApplicationCore.Interfaces.Repository;

public interface IPositionRepository : IGenericRepository<Position>
{
    Task<bool> ExistsAsync(string name);
    Task<List<Position>> GetAllAsync();
}