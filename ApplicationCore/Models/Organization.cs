using System.Collections.Generic;
namespace ApplicationCore.Models;

public class Organization
{
    public List<Person> OrganizationMembers { get; set; } = [];
}
