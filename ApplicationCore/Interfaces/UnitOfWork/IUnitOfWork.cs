using ApplicationCore.Interfaces.Repository;
namespace ApplicationCore.Interfaces.UnitOfWork;

public interface IUnitOfWork
{
    public IContactRepository Contacts { get; }
    public IOrganizationRepository Organizations { get; }
    public IPersonRepository Persons { get; }
    public ICompanyRepository Companies { get; }
    public IRemovedContactRepository RemovedContacts { get; }
    public IPositionRepository Positions { get; }

    public Task<int> SaveChangesAsync();
    public Task BeginTransactionAsync();
    public Task CommitTransactionAsync();
    public Task RollbackTransactionAsync();
}
