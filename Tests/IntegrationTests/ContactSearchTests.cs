using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using ApplicationCore.ValueObjects.Pesel;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.IntegrationTests;

public class ContactSearchTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Search_ByEmailDomain_ShouldReturnMatchingContacts()
    {
        // ARRANGE
        using var scope = factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var adam = new Person
        {
            Id = Guid.NewGuid(),
            Name = "Adam",
            PESEL = new Pesel("44051401359"),
            Email = "adam@wsei.pl",
            CreatedById = Guid.NewGuid()
        };
        unitOfWork.Contacts.Add(adam);
        await unitOfWork.SaveChangesAsync();

        // ACT
        var results = unitOfWork.Contacts.FindByEmailDomain("wsei.pl").ToList();

        // ASSERT
        Assert.Contains(results, c => c.Id == adam.Id);
    }
}