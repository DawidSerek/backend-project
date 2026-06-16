using ApplicationCore.Models;
using ApplicationCore.ValueObjects.Nip;
using ApplicationCore.ValueObjects.Pesel;
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
    public DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Contact config
        modelBuilder.Entity<Contact>()
            .HasDiscriminator<string>("ContactType")
            .HasValue<Person>("Person")
            .HasValue<Organization>("Organization")
            .HasValue<Company>("Company");

        // Conversion configs
        modelBuilder.Entity<Person>()
            .Property(p => p.PESEL)
            .HasConversion(new PeselConverter(), new PeselComparer())
            .HasMaxLength(11);

        modelBuilder.Entity<Company>()
            .Property(c => c.Nip)
            .HasConversion(new NipConverter(), new NipComparer());

        modelBuilder.Entity<Contact>()
            .Property(c => c.PhoneNumber)
            .HasConversion(new PhoneNumberConverter(), new PhoneNumberComparer())
            .HasMaxLength(16);

        // Delete behaviors
        modelBuilder.Entity<Person>()
            .HasOne(p => p.Organization)
            .WithMany(o => o.OrganizationMembers)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Person>()
            .HasOne(p => p.Employer)
            .WithMany(e => e.Employees)
            .OnDelete(DeleteBehavior.SetNull);

        base.OnModelCreating(modelBuilder);
    }
}
