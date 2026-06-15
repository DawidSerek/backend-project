using ApplicationCore.ValueObjects.Nip;

namespace Tests.UnitTests;

public class NipTests
{
    [Theory]
    [InlineData("5261040828", true)]
    [InlineData("1234567891", false)]
    [InlineData("123-456-78-91", false)]
    [InlineData("0000000000", false)]
    [InlineData("5260250274", true)]
    public void NIP_ShouldValidateChecksum(string input, bool expected)
    {
        var ok = NipParser.TryParse(input, out _);
        Assert.Equal(expected, ok);
    }

    [Fact]
    public void NIP_OfficeNumber_ShouldReturnFirstThreeDigits()
    {
        var nip = new Nip("5261040828");
        Assert.Equal("526", nip.OfficeNumber);
    }

    [Fact]
    public void NIP_OfficeName_ShouldReturnFullName()
    {
        var nip = new Nip("5261040828");
        Assert.NotEqual("Unknown office (526)", nip.OfficeName);
    }

    [Fact]
    public void NIP_Empty_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new Nip(""));
    }
}
