using ApplicationCore.Models;

namespace ApplicationCore.Services.DeduplicationStrategy;

public interface IDeduplicationStrategy
{
    bool IsMatch(Contact a, Contact b, DeduplicationConfigDto config);
    Dictionary<string, double> GetScores(Contact a, Contact b, DeduplicationConfigDto config);
}

public interface IDeduplicationStrategyService
{
    Task<DeduplicationReportDto> FindDuplicatesAsync(DeduplicationConfigDto config);
    Task<DeduplicationReportDto> RemoveDuplicatesAsync(DeduplicationConfigDto config, Guid userId);
}

public enum DeduplicationStrategyOptions
{
    Exact,
    Fuzzy,
    Both
}

