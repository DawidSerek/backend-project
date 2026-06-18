namespace ApplicationCore.Services.DeduplicationStrategy;

public record DeduplicationConfigDto(
    double Threshold,
    List<string> Properties,
    DeduplicationStrategyOptions Strategy
);

public record DeduplicationReportDto(
    int TotalContactsScanned,
    List<DuplicatePairDto> Duplicates,
    int RemovedCount
);

public record DuplicatePairDto(
    Guid Id1, Guid Id2,
    string Name1, string Name2,
    double Score,
    Dictionary<string, double> PerPropertyScores
);