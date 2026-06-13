using ApplicationCore.ValueObjects.Pesel;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Configurations.ValueConverters;

public class PeselConverter : ValueConverter<PESEL, string>
{
    public PeselConverter()
        : base(
            pesel => pesel.Value,
            value => new PESEL(value)
        )
    { }
}
