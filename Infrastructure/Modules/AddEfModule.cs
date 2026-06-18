using ApplicationCore.Interfaces.Import;
using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Services.ContactFactory;
using ApplicationCore.Services.DeduplicationStrategy;
using ApplicationCore.Services.Ef.CompanyService;
using ApplicationCore.Services.Ef.ContactService;
using ApplicationCore.Services.Ef.ImportService;
using ApplicationCore.Services.Ef.InteractionService;
using ApplicationCore.Services.Ef.OrganizationService;
using ApplicationCore.Services.Ef.PersonService;
using Infrastructure.EntityFramework.Context;
using Infrastructure.EntityFramework.Repository;
using Infrastructure.EntityFramework.UnitOfWork;
using Infrastructure.Identity;
using Infrastructure.Import;
using Microsoft.AspNetCore.Identity;
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
        // Add repositories & Unit of work
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IInteractionRepository, InteractionRepository>();
        services.AddScoped<IRemovedContactRepository, RemovedContactRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services

        // Contact
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IContactFactory, ContactFactory>();
        services.AddScoped<IContactImportService, ContactImportService>();
        services.AddScoped<IContactFileParser, CsvContactParser>();
        services.AddScoped<IContactFileParser, JsonContactParser>();
        services.AddScoped<IContactImportFactory, ContactImportFactory>();

        // Contact - implementations
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IAdminService, AdminService>();

        // Interaction
        services.AddScoped<IInteractionService, InteractionService>();

        // Authentication
        services.AddSingleton<JwtSettings>();
        services.AddScoped<IAuthService, AuthService>();

        // Other services
        services.AddScoped<IDeduplicationStrategyService, DeduplicationStrategyService>();

        // Add database context and connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var sqliteBuilder = new SqliteConnectionStringBuilder(connectionString);
        if (!string.IsNullOrEmpty(sqliteBuilder.DataSource) && !Path.IsPathRooted(sqliteBuilder.DataSource))
        {
            sqliteBuilder.DataSource = Path.Combine(contentRootPath, sqliteBuilder.DataSource);
        }
        services.AddDbContext<AppDbContext>(opts => opts.UseSqlite(sqliteBuilder.ConnectionString));

        // Add identity
        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Lockout.MaxFailedAccessAttempts = 5;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}
