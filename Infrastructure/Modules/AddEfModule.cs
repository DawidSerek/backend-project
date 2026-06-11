using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using ApplicationCore.Services.Ef;
using Infrastructure.EntityFramework.Context;
using Infrastructure.EntityFramework.Repository;
using Infrastructure.EntityFramework.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Modules;

public static class EfModule
{
    public static IServiceCollection RegisterEfModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<AppDbContext>();

        services.AddScoped<IContactService, ContactService>();

        return services;
    }
}
