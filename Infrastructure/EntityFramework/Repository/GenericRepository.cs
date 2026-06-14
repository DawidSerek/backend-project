using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Primitives;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repository;

public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> where T : EntityBase
{
    protected DbSet<T> DbSet => context.Set<T>();

    public T? FindById(Guid id)
        => DbSet.FirstOrDefault(e => e.Id == id);

    public IEnumerable<T> GetAll()
        => [.. DbSet];

    public T Add(T entity)
    {
        DbSet.Add(entity);
        return entity;
    }

    public T Update(T entity)
    {
        context.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    public void RemoveById(Guid id)
    {
        var existing = DbSet.Find(id);
        if (existing is not null)
            DbSet.Remove(existing);
    }
}
