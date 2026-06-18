using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Seed;

public static class PositionSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IPositionRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var names = new[] {
            "Developer", "Senior Developer", "Tech Lead", "Manager", 
            "Project Manager", "QA Engineer", "DevOps Engineer", "Designer",
            "Product Owner", "Scrum Master", "Architect", "CTO", "CEO",
            "Marketing Specialist", "Sales Representative", "Account Manager",
            "Customer Support", "Data Analyst", "Data Scientist", "HR Specialist"
        };

        foreach (var name in names)
        {
            if (!await repo.ExistsAsync(name))
            {
                uow.Positions.Add(new Position { Id = Guid.NewGuid(), Name = name });
            }
        }
        await uow.SaveChangesAsync();
    }
}