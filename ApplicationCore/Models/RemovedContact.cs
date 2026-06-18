using ApplicationCore.Primitives;

namespace ApplicationCore.Models;

public class RemovedContact : EntityBase
{
    public Guid OriginalContactId { get; set; }
    public string Type { get; set; } = "";
    public string JsonSnapshot { get; set; } = "";
    public DateTime RemovedAt { get; set; } = DateTime.UtcNow;
    public string RemovedById { get; set; } = "";
    public string? Reason { get; set; }
}
