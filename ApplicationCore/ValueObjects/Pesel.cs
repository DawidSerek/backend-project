namespace ApplicationCore.ValueObjects;

public record PESEL
{
    public string Value { get; init; }

    private static readonly int[] Weights = [1, 3, 7, 9, 1, 3, 7, 9, 1, 3];

    public PESEL(string pesel)
    {
        if (pesel.Length != 11)
            throw new ArgumentException("PESEL must be 11 digits long");

        if (!pesel.All(char.IsDigit))
            throw new ArgumentException("PESEL must contain only digits");

        Value = pesel;

        if (!CheckSum(pesel))
            throw new ArgumentException("Invalid PESEL checksum");
    }

    private static bool CheckSum(string p)
    {
        int sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (p[i] - '0') * Weights[i];

        return (p[10] - '0') == (10 - (sum % 10)) % 10;
    }
}

public static class PeselDateValidator
{
    public static bool HasValidBirthDate(string pesel)
    {
        int yearOffset = ((pesel[0] - '0') * 10) + (pesel[1] - '0');
        int monthDigits = ((pesel[2] - '0') * 10) + (pesel[3] - '0');
        int day = ((pesel[4] - '0') * 10) + (pesel[5] - '0');

        int century = monthDigits switch
        {
            >= 1 and <= 12 => 1900,
            >= 21 and <= 32 => 2000,
            >= 41 and <= 52 => 1800,
            >= 61 and <= 72 => 2100,
            >= 81 and <= 92 => 2200,
            _ => -1
        };

        if (century == -1) return false;

        int actualYear = century + yearOffset;
        int actualMonth = monthDigits - (century switch
        {
            1900 => 0,
            2000 => 20,
            1800 => 40,
            2100 => 60,
            2200 => 80,
            _ => 0
        });

        try
        {
            var verifiedDate = new DateTime(actualYear, actualMonth, day);
            return true;
        }
        catch (ArgumentOutOfRangeException)
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

public enum Gender { Male, Female }

public static class PeselGenderExtractor
{
    public static Gender Extract(string pesel)
        => (pesel[10] - '0') % 2 == 1 ? Gender.Male : Gender.Female;
}
