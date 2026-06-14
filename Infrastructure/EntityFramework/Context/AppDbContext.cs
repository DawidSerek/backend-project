using ApplicationCore.Models;
using Infrastructure.Configurations.ValueComparers;
using Infrastructure.Configurations.ValueConverters;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.EntityFramework.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Organization> Organizations { get; set; }

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
