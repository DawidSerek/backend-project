using System.Collections.Generic;
using ApplicationCore.Primitives;
namespace ApplicationCore.Models;

public class Organization : Contact
{
    public List<Person> OrganizationMembers { get; set; } = [];
}
