using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using ApplicationCore.ValueObjects.Nip;
using ApplicationCore.ValueObjects.Pesel;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Tests.IntegrationTests;

public class CompanyControllerTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetEmployees_ShouldReturnSortedList()
    {
        // ARRANGE
        using var scope = factory.Services.CreateScope();
        var companyRepo = scope.ServiceProvider.GetRequiredService<ICompanyRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var acme = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Acme",
            Nip = new Nip("5261040828"),
            CreatedById = Guid.NewGuid()
        };
        companyRepo.Add(acme);

        for (var i = 0; i < 3; i++)
        {
            unitOfWork.Persons.Add(new Person
            {
                Id = Guid.NewGuid(),
                Name = $"Employee {i}",
                PESEL = new Pesel("44051401359"),
                Email = $"e{i}@acme.test",
                CreatedById = Guid.NewGuid(),
                Employer = acme
            });
        }
        await unitOfWork.SaveChangesAsync();

        // ACT
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/companies/{acme.Id}/employees?sortBy=lastName&desc=true");

        // ASSERT
        response.EnsureSuccessStatusCode();
        var employees = await response.Content.ReadFromJsonAsync<List<PersonDto>>();
        Assert.NotNull(employees);
        Assert.Equal(3, employees.Count);
    }

    public record PersonDto(Guid Id, string Name, string? Email);
}
