using ApplicationCore.Primitives;

namespace ApplicationCore.Models;

public class Position : EntityBase
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
