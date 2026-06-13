using ApplicationCore.ValueObjects.Pesel;
namespace ApplicationCore.Models;

public class Person : Contact
{
    public required PESEL PESEL { get; set; }
    public Organization? Organization { get; set; }
}
