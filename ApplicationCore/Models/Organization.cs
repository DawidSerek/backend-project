using System.Collections.Generic;
using ApplicationCore.Primitives;
namespace ApplicationCore.Models;

public class Organization : EntityBase
{
    public List<Person> OrganizationMembers { get; set; } = [];
}
