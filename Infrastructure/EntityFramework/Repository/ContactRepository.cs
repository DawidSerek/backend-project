using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Models;
using Infrastructure.EntityFramework.Context;

namespace Infrastructure.EntityFramework.Repository;

public class ContactRepository(AppDbContext context) : GenericRepository<Contact>(context), IContactRepository
{
}
