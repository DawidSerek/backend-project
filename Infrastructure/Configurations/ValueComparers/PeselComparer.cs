using ApplicationCore.ValueObjects.Pesel;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Configurations.ValueComparers;

public class PeselComparer: ValueComparer<Pesel>
{
    public PeselComparer() : base(
        equalsExpression: (a, b) => a!.Value == b!.Value,
        hashCodeExpression: pesel => pesel.Value.GetHashCode(),
        snapshotExpression: pesel => new Pesel(pesel.Value)
    )
    {}
}
