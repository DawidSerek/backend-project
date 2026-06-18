using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Models;
using Infrastructure.EntityFramework.Context;

namespace Infrastructure.EntityFramework.Repository;

public class RemovedContactRepository(AppDbContext context) : GenericRepository<RemovedContact>(context), IRemovedContactRepository;
