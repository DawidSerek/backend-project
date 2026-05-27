using ApplicationCore.ValueObjects;
namespace ApplicationCore.Models;

public class Person : Contact
{
    public required PESEL PESEL { get; set; }
}
