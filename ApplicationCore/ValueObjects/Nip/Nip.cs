namespace ApplicationCore.ValueObjects.Nip;

public record Nip
{
    public string Value { get; init; }
    private static readonly int[] Weights = [6, 5, 7, 2, 3, 4, 5, 6, 7];

    private static readonly Dictionary<string, string> OfficeNames = new()
    {
        ["101"] = "Urząd Skarbowy Warszawa-Mokotów",
        ["102"] = "Urząd Skarbowy Warszawa-Praga Południe",
        ["121"] = "I Urząd Skarbowy Warszawa-Śródmieście",
        ["122"] = "II Urząd Skarbowy Warszawa-Śródmieście",
        ["141"] = "Urząd Skarbowy Kraków-Śródmieście",
        ["142"] = "Urząd Skarbowy Kraków-Podgórze",
        ["201"] = "Urząd Skarbowy Łódź-Śródmieście",
        ["301"] = "Urząd Skarbowy Wrocław-Fabryczna",
        ["401"] = "Urząd Skarbowy Poznań-Nowe Miasto",
        ["501"] = "Urząd Skarbowy Gdańsk-Południe",
        ["526"] = "Urząd Skarbowy Gdynia-Śródmieście",
        ["601"] = "Urząd Skarbowy Szczecin-Prawobrzeże",
        ["701"] = "Urząd Skarbowy Bydgoszcz-Centrum",
        ["801"] = "Urząd Skarbowy Lublin-Południe",
        ["901"] = "Urząd Skarbowy Katowice-Piotrowice"
    };

    public Nip(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("NIP cannot be empty", nameof(value));

        var digits = value.Replace("-", "").Replace(" ", "");

        if (digits.Length != 10 || !digits.All(char.IsDigit))
            throw new ArgumentException("NIP must be exactly 10 digits", nameof(value));

        if (digits.Distinct().Count() == 1)
            throw new ArgumentException("NIP cannot consist of a single repeated digit", nameof(value));

        if (!IsCheckDigitValid(digits))
            throw new ArgumentException("NIP has an invalid check digit", nameof(value));

        Value = digits;
    }

    private static bool IsCheckDigitValid(string digits)
    {
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (digits[i] - '0') * Weights[i];

        int check = sum % 11;
        if (check == 10) return false;
        return check == digits[9] - '0';
    }

    public string OfficeNumber => Value[..3];
    public string OfficeName =>
        OfficeNames.TryGetValue(OfficeNumber, out var name)
            ? name
            : $"Unknown office ({OfficeNumber})";
}