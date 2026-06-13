using ApplicationCore.Models;
using Infrastructure.Configurations.ValueComparers;
using Infrastructure.Configurations.ValueConverters;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.EntityFramework.Context;

public class AppDbContext : IdentityDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=backend-project.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Contact config
        modelBuilder.Entity<Contact>()
            .HasDiscriminator<string>("ContactType")
            .HasValue<Person>("Person")
            .HasValue<Organization>("Organization");

        // Person config
        modelBuilder.Entity<Person>()
            .Property(p => p.PESEL)
            .HasConversion(new PeselConverter(), new PeselComparer())
            .HasMaxLength(11);

        modelBuilder.Entity<Person>()
            .HasOne(p => p.Organization)
            .WithMany(o => o.OrganizationMembers)
            .OnDelete(DeleteBehavior.SetNull);

        base.OnModelCreating(modelBuilder);
    }
}
