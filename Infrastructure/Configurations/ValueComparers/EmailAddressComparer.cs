using ApplicationCore.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Configurations.ValueComparers;

public class EmailAddressComparer : ValueComparer<EmailAddress>
{
    public EmailAddressComparer()
        : base(
            (a, b) => a!.Value == b!.Value,
            e => e.Value.GetHashCode(),
            e => new EmailAddress(e.Value)
        )
    { }
}