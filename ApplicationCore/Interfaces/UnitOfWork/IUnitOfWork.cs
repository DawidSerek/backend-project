using ApplicationCore.Interfaces.Repository;
namespace ApplicationCore.Interfaces.UnitOfWork;

public interface IUnitOfWork
{
    public IContactRepository Contacts { get; }
    public IOrganizationRepository Organizations { get; }

    public Task<int> SaveChangesAsync();
    public Task BeginTransactionAsync();
    public Task CommitTransactionAsync();
    public Task RollbackTransactionAsync();
}
