using ApplicationCore.Models;
using ApplicationCore.Models.Interactions;
using ApplicationCore.Models.Security;
using ApplicationCore.ValueObjects.Nip;
using ApplicationCore.ValueObjects.Pesel;
using Infrastructure.Configurations.ValueComparers;
using Infrastructure.Configurations.ValueConverters;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.EntityFramework.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Interaction> Interactions { get; set; }
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TPH
        modelBuilder.Entity<Contact>()
            .HasDiscriminator<string>("ContactType")
            .HasValue<Person>("Person")
            .HasValue<Organization>("Organization")
            .HasValue<Company>("Company");

        modelBuilder.Entity<Interaction>()
            .HasDiscriminator<string>("InteractionType")
            .HasValue<EmailInteraction>("Email")
            .HasValue<SmsInteraction>("Sms")
            .HasValue<MeetingInteraction>("Meeting");

        // Indexing
        modelBuilder.Entity<Interaction>()
            .HasIndex(i => i.Date);

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

        modelBuilder.Entity<Contact>()
            .Property(c => c.Email)
            .HasConversion(new EmailAddressConverter(), new EmailAddressComparer())
            .HasMaxLength(256);

        // Delete behaviors
        modelBuilder.Entity<Person>()
            .HasOne(p => p.Organization)
            .WithMany(o => o.OrganizationMembers)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Person>()
            .HasOne(p => p.Employer)
            .WithMany(e => e.Employees)
            .OnDelete(DeleteBehavior.SetNull);

        // AppUser constraints
        modelBuilder.Entity<AppUser>(e => {
            e.Property(u => u.FirstName).HasMaxLength(100);
            e.Property(u => u.LastName).HasMaxLength(100);
            e.HasIndex(u => u.Email).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
