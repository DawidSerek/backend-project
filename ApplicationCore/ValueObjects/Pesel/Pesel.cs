namespace ApplicationCore.ValueObjects.Pesel;

public record Pesel
{
    public string Value { get; init; }
    public DateOnly BirthDate { get => PeselDateValidator.Decode(this.Value); }
    public Gender Gender { get => PeselGenderExtractor.Extract(this.Value); }

    public Pesel(string pesel)
    {
        if (pesel.Length != 11)
            throw new ArgumentException("PESEL must be 11 digits long");

        if (!pesel.All(char.IsDigit))
            throw new ArgumentException("PESEL must contain only digits");

        Value = pesel;

        if (!PeselCheckDigitCalculator.IsValid(pesel))
            throw new ArgumentException("Invalid PESEL checksum");
    }
}

public enum Gender { Male, Female }

