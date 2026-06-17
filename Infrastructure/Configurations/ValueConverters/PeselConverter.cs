using ApplicationCore.ValueObjects.Pesel;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Configurations.ValueConverters;

public class PeselConverter : ValueConverter<Pesel, string>
{
    public PeselConverter()
        : base(
            pesel => pesel.Value,
            v => new Pesel(v)
        )
    { }
}
