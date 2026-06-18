using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using ApplicationCore.Models.Dto;
using ApplicationCore.ValueObjects;
using ApplicationCore.ValueObjects.Pesel;
using Infrastructure.EntityFramework.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.IntegrationTests;

public class DeduplicationControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private const string AdminEmail = "admin@local";
    private const string AdminPassword = "Admin123!";

    private readonly WebApplicationFactory<Program> _factory;

    public DeduplicationControllerTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private async Task<string> LoginAsAdminAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginDto(AdminEmail, AdminPassword));

        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(auth);
        return auth.AccessToken;
    }

    private async Task<(Guid KeepId, Guid DupId)> SeedDuplicatesAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await db.Database.ExecuteSqlRawAsync("DELETE FROM Interactions");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM Contacts");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM RemovedContacts");

        var first = new Person
        {
            Id = Guid.NewGuid(),
            Name = "Deduplication Test Person",
            PESEL = new Pesel("44051401359"),
            Email = new EmailAddress("dedup.test@example.com"),
            PhoneNumber = new PhoneNumber("+48500100200"),
            CreatedById = Guid.NewGuid()
        };
        var second = new Person
        {
            Id = Guid.NewGuid(),
            Name = "Deduplication Test Person",
            PESEL = new Pesel("44051401359"),
            Email = new EmailAddress("dedup.test@example.com"),
            PhoneNumber = new PhoneNumber("+48500100200"),
            CreatedById = Guid.NewGuid()
        };

        var keepId = first.Id.CompareTo(second.Id) < 0 ? first.Id : second.Id;
        var dupId = first.Id.CompareTo(second.Id) < 0 ? second.Id : first.Id;

        uow.Contacts.Add(first);
        uow.Contacts.Add(second);
        await uow.SaveChangesAsync();

        return (keepId, dupId);
    }

    [Fact]
    public async Task Find_WithoutAuth_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/deduplication/find",
            new { threshold = 0.5, properties = new[] { "name" }, strategy = 0 });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Find_WithExactMatch_ReturnsDuplicatePair()
    {
        var (keepId, dupId) = await SeedDuplicatesAsync();

        var client = _factory.CreateClient();
        var token = await LoginAsAdminAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync(
            "/api/deduplication/find",
            new
            {
                threshold = 1.0,
                properties = new[] { "name", "email", "phonenumber" },
                strategy = 0
            });

        response.EnsureSuccessStatusCode();
        var report = await response.Content.ReadFromJsonAsync<DeduplicationReport>();
        Assert.NotNull(report);

        var pair = report.Duplicates.FirstOrDefault(p =>
            (p.Id1 == keepId && p.Id2 == dupId) || (p.Id1 == dupId && p.Id2 == keepId));
        Assert.NotEqual(default, pair);
        Assert.Equal(1.0, pair.Score);
    }

    [Fact]
    public async Task Find_WithInvalidThreshold_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();
        var token = await LoginAsAdminAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync(
            "/api/deduplication/find",
            new { threshold = 1.5, properties = new[] { "name" }, strategy = 0 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Remove_WithFuzzyMatch_RemovesDuplicateAndCreatesSnapshot()
    {
        var (keepId, dupId) = await SeedDuplicatesAsync();

        var client = _factory.CreateClient();
        var token = await LoginAsAdminAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync(
            "/api/deduplication/remove",
            new
            {
                threshold = 0.8,
                properties = new[] { "name", "email", "phonenumber" },
                strategy = 1
            });

        response.EnsureSuccessStatusCode();
        var report = await response.Content.ReadFromJsonAsync<DeduplicationReport>();
        Assert.NotNull(report);
        Assert.True(report.RemovedCount >= 1);

        using var verifyScope = _factory.Services.CreateScope();
        var db = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();

        Assert.Null(db.Contacts.Find(dupId));
        Assert.NotNull(db.Contacts.Find(keepId));

        var removed = db.RemovedContacts.SingleOrDefault(r => r.OriginalContactId == dupId);
        Assert.NotNull(removed);
        Assert.Equal("Person", removed.Type);
    }

    private record DeduplicationReport(
        int TotalContactsScanned,
        List<DuplicatePair> Duplicates,
        int RemovedCount);

    private record DuplicatePair(
        Guid Id1,
        Guid Id2,
        string Name1,
        string Name2,
        double Score,
        Dictionary<string, double> PerPropertyScores);
}
