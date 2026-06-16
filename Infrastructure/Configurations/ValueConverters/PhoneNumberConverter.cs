using ApplicationCore.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Configurations.ValueConverters;

public class PhoneNumberConverter : ValueConverter<PhoneNumber?, string>
{
    public PhoneNumberConverter()
        : base(
            p => p == null ? null : p.Value,
            v => v == null ? null : new PhoneNumber(v))
    { }
}
