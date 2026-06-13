namespace ApplicationCore.ValueObjects.Pesel;

public static class PeselDateValidator
{
    public static DateOnly Decode(string pesel)
    {
        int yearOffset = ((pesel[0] - '0') * 10) + (pesel[1] - '0');
        int monthDigits = ((pesel[2] - '0') * 10) + (pesel[3] - '0');
        int day = ((pesel[4] - '0') * 10) + (pesel[5] - '0');

        var (century, monthOffset) = monthDigits switch
        {
            >= 1 and <= 12  => (1900, 0),
            >= 21 and <= 32 => (2000, 20),
            >= 41 and <= 52 => (1800, 40),
            >= 61 and <= 72 => (2100, 60),
            >= 81 and <= 92 => (2200, 80),
            _ => throw new ArgumentException("PESEL month encoding is invalid")
        };

        return new DateOnly(century + yearOffset, monthDigits - monthOffset, day);
    }

    public static bool HasValidBirthDate(string pesel)
    {
        try
        {
            Decode(pesel);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public static class PeselCheckDigitCalculator
{
    private static readonly int[] Weights = [1, 3, 7, 9, 1, 3, 7, 9, 1, 3];

    public static int CalculateCheckDigit(string pesel)
    {
        if (pesel is null || pesel.Length != 10)
            throw new ArgumentException("PESEL must be exactly 10 digits", nameof(pesel));

        int sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (pesel[i] - '0') * Weights[i];

        return (10 - (sum % 10)) % 10;
    }

    public static bool IsValid(string pesel) =>
        CalculateCheckDigit(pesel[..10]) == pesel[10] - '0';
}

public static class PeselGenderExtractor
{
    public static Gender Extract(string pesel)
        => (pesel[10] - '0') % 2 == 1 ? Gender.Male : Gender.Female;
}

