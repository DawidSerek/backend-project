using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Services.Ef.ContactService;
using ApplicationCore.Services.Ef.OrganizationService;
using ApplicationCore.Services.Ef.PersonService;
using Infrastructure.EntityFramework.Context;
using Infrastructure.EntityFramework.Repository;
using Infrastructure.EntityFramework.UnitOfWork;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Modules;

public static class EfModule
{
    public static IServiceCollection RegisterEfModule(
        this IServiceCollection services,
        IConfiguration configuration,
        string contentRootPath)
    {
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var sqliteBuilder = new SqliteConnectionStringBuilder(connectionString);
        if (!string.IsNullOrEmpty(sqliteBuilder.DataSource) && !Path.IsPathRooted(sqliteBuilder.DataSource))
        {
            sqliteBuilder.DataSource = Path.Combine(contentRootPath, sqliteBuilder.DataSource);
        }
        services.AddDbContext<AppDbContext>(opts => opts.UseSqlite(sqliteBuilder.ConnectionString));

        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IPersonService, PersonService>();

        return services;
    }
}
