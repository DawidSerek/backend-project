using ApplicationCore.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Configurations.ValueComparers;

public class PhoneNumberComparer : ValueComparer<PhoneNumber?>
{
    public PhoneNumberComparer()
        : base(
            equalsExpression: (a, b) => a!.Value == b!.Value,
            hashCodeExpression: a => a.Value.GetHashCode(),
            snapshotExpression: a => a == null ? null : new PhoneNumber(a.Value))
    { }
}
