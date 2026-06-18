
using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Models;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repository;

public class TagRepository(AppDbContext context) 
    : GenericRepository<Tag>(context), ITagRepository
{
    public Task<Tag?> GetByNameAsync(string name) =>
        DbSet.FirstOrDefaultAsync(t => t.Name == name);

    public Task<bool> ExistsAsync(string name) =>
        DbSet.AnyAsync(t => t.Name == name);
}