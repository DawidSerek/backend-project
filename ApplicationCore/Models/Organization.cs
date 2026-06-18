using System.Collections.Generic;
using ApplicationCore.Primitives;
namespace ApplicationCore.Models;

public class Organization : Contact
{
    public List<Person> OrganizationMembers { get; set; } = [];
    public string? Krs { get; set; }
    public string? Website { get; set; }
}