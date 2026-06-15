using ApplicationCore.ValueObjects.Nip;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Configurations.ValueComparers;

public class NipComparer : ValueComparer<Nip>
{
    public NipComparer() : base(
        equalsExpression: (a, b) => a!.Value == b!.Value,
        hashCodeExpression: nip => nip.Value.GetHashCode(),
        snapshotExpression: nip => new Nip(nip.Value)
    )
    {}
}
