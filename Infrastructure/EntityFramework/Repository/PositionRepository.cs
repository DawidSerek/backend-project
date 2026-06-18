using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Models;
using Infrastructure.EntityFramework.Context;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.EntityFramework.Repository;

public class PositionRepository(
    AppDbContext context,
    IMemoryCache cache) : GenericRepository<Position>(context), IPositionRepository
{
    private const string CacheKey = "all_positions";

    public async Task<bool> ExistsAsync(string name)
    {
        var all = await GetAllAsync();
        return all.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<Position>> GetAllAsync()
    {
        if (cache.TryGetValue(CacheKey, out List<Position>? cached) && cached is not null)
            return cached;

        var positions = context.Positions.ToList();
        cache.Set(CacheKey, positions, TimeSpan.FromMinutes(10));
        return positions;
    }

    public async Task InvalidateCacheAsync()
    {
        cache.Remove(CacheKey);
        await Task.CompletedTask;
    }
}