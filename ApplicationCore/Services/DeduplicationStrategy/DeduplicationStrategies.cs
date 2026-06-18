using ApplicationCore.Models;
using ApplicationCore.Services.JaroWinkler;

namespace ApplicationCore.Services.DeduplicationStrategy;

public class ExactStrategy : IDeduplicationStrategy
{
    public bool IsMatch(Contact a, Contact b, DeduplicationConfigDto config)
    {
        foreach (var prop in config.Properties)
        {
            var valA = GetProperty(a, prop);
            var valB = GetProperty(b, prop);
            if (valA is null || valB is null) continue;
            if (!valA.Equals(valB, StringComparison.OrdinalIgnoreCase)) return false;
        }
        return true;
    }

    public Dictionary<string, double> GetScores(Contact a, Contact b, DeduplicationConfigDto config)
    {
        var result = new Dictionary<string, double>();
        foreach (var prop in config.Properties)
        {
            var valA = GetProperty(a, prop) ?? "";
            var valB = GetProperty(b, prop) ?? "";
            result[prop] = valA.Equals(valB, StringComparison.OrdinalIgnoreCase) ? 1.0 : 0.0;
        }
        return result;
    }

    private static string? GetProperty(Contact c, string prop) => prop.ToLower() switch
    {
        "name" => c.Name,
        "email" => c.Email?.Value,
        "phonenumber" => c.PhoneNumber?.Value,
        _ => null
    };
}

public class FuzzyStrategy : IDeduplicationStrategy
{
    public bool IsMatch(Contact a, Contact b, DeduplicationConfigDto config)
    {
        var scores = GetScores(a, b, config);
        if (scores.Count == 0) return false;
        return scores.Values.Average() >= config.Threshold;
    }

    public Dictionary<string, double> GetScores(Contact a, Contact b, DeduplicationConfigDto config)
    {
        var result = new Dictionary<string, double>();
        foreach (var prop in config.Properties)
        {
            var valA = Normalize(GetProperty(a, prop));
            var valB = Normalize(GetProperty(b, prop));
            result[prop] = JaroWinklerService.Similarity(valA, valB);
        }
        return result;
    }

    private static string? Normalize(string? s) => s?.ToLowerInvariant().Trim();

    private static string? GetProperty(Contact c, string prop) => prop.ToLower() switch
    {
        "name" => c.Name,
        "email" => c.Email?.Value,
        "phonenumber" => c.PhoneNumber?.Value,
        _ => null
    };
}

