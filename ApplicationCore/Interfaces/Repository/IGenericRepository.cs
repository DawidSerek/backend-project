using ApplicationCore.Primitives;

namespace ApplicationCore.Interfaces.Repository;

public interface IGenericRepository<T> where T : EntityBase
{
    T? FindById(Guid id);
    IEnumerable<T> GetAll();

    T Add(T entity);
    T Update(T entity);
    void RemoveById(Guid id);
}