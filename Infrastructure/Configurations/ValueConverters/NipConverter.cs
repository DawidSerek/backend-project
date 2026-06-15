using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ApplicationCore.ValueObjects.Nip;

namespace Infrastructure.Configurations.ValueConverters;

public class NipConverter : ValueConverter<Nip, string>
{
    public NipConverter()
        : base(
            pesel => pesel.Value,
            value => new Nip(value)
        )
    { }
}