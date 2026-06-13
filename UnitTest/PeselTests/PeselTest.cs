using ApplicationCore.Models;
using ApplicationCore.ValueObjects.Pesel;

namespace UnitTest.PeselTests;

public class PeselTest
{
    [Fact]
    public void Pesel_Is_Too_Short()
    {
        Assert.ThrowsAny<ArgumentException>(() => new PESEL("123"));
    }

    [Fact]
    public void Pesel_Doesnt_Contain_Only_Digits()
    {
        Assert.ThrowsAny<ArgumentException>(() => new PESEL("77a82336912"));
    }

    [Fact]
    public void Pesel_Comparasion_Works()
    {
        var pesel1 = new PESEL("77082336912");
        var pesel2 = new PESEL("81072381949");

        Assert.False(pesel1 == pesel2);
    }

    [Fact]
    public void Pesel_Checksum_Works()
    {
        Assert.ThrowsAny<ArgumentException>(() => new PESEL("12312312312"));
    }

    [Fact]
    public void Pesel_Assignement_Works()
    {
        var pesel1 = new PESEL("72052191241");
        var pesel2 = pesel1;

        Assert.True(pesel1 == pesel2);
    }
}
