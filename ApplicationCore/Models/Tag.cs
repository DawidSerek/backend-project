using ApplicationCore.Primitives;

namespace ApplicationCore.Models;

public class Tag : EntityBase
{
    public required string Name { get; set; }
    public string? Color { get; set; }
    public List<Contact> Contacts { get; set; } = [];
}