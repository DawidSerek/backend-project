namespace ApplicationCore.ValueObjects;

using System.Text.RegularExpressions;

public record PhoneNumber
{
    public string Value { get; init; }
    public string CountryCode { get => DetectCountry(Value).code; }
    public string CountryName { get => DetectCountry(Value).name; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty");

        var normalized = Normalize(value, defaultCountryCode: "48");

        if (!Regex.IsMatch(normalized, @"^\+[1-9]\d{6,14}$"))
            throw new ArgumentException($"Phone number '{value}' is not a valid E.164 number");

        Value = normalized;
    }

    private static readonly Dictionary<string, (string Code, string Name)> Countries = new()
    {
        ["48"] = ("PL", "Poland"),
        ["1"]  = ("US", "United States"),
        ["44"] = ("GB", "United Kingdom"),
        ["49"] = ("DE", "Germany"),
        ["33"] = ("FR", "France"),
        ["34"] = ("ES", "Spain"),
        ["39"] = ("IT", "Italy"),
        ["31"] = ("NL", "Netherlands"),
        ["420"] = ("CZ", "Czech Republic"),
        ["421"] = ("SK", "Slovakia")
    };

    private static string Normalize(string input, string defaultCountryCode)
    {
        var s = input.Trim();

        s = Regex.Replace(s, @"[\s\-\(\)\.]", "");

        if (s.StartsWith("00"))
            s = "+" + s[2..];

        if (!s.StartsWith("+"))
            s = "+" + defaultCountryCode + s;

        return s;
    }

    private static (string code, string name) DetectCountry(string e164)
    {
        if (string.IsNullOrEmpty(e164) || !e164.StartsWith('+'))
            return ("??", "Unknown");

        var digits = e164[1..];

        foreach (var len in new[] { 3, 2, 1 })
        {
            if (digits.Length >= len && Countries.TryGetValue(digits[..len], out var c))
                return c;
        }

        return ("??", "Unknown");
    }
}
