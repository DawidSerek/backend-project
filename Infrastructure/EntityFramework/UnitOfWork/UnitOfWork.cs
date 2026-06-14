using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Interfaces.UnitOfWork;
using Infrastructure.EntityFramework.Context;

namespace Infrastructure.EntityFramework.UnitOfWork;

public class UnitOfWork(
    IContactRepository contactRepository,
    IOrganizationRepository organizationRepository,
    IPersonRepository personRepository,
    AppDbContext context
) : IUnitOfWork
{
    public IContactRepository Contacts { get; } = contactRepository;
    public IOrganizationRepository Organizations { get; } = organizationRepository;
    public IPersonRepository Persons { get; } = personRepository;

    private readonly AppDbContext _context = context;

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
    public Task BeginTransactionAsync()
    {
        return _context.Database.BeginTransactionAsync();
    }
    public Task CommitTransactionAsync()
    {
        return _context.Database.CommitTransactionAsync();
    }
    public Task RollbackTransactionAsync()
    {
        return _context.Database.RollbackTransactionAsync();
    }
}
