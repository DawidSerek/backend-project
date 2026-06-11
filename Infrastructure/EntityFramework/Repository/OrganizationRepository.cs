using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Models;
using Infrastructure.EntityFramework.Context;

namespace Infrastructure.EntityFramework.Repository;

public class OrganizationRepository(AppDbContext context) : GenericRepository<Organization>(context), IOrganizationRepository
{
}
