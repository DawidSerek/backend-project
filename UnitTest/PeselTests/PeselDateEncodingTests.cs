using ApplicationCore.Models;
using ApplicationCore.ValueObjects.Pesel;

namespace UnitTest.PeselTests;

public class PeselDateEncodingTests
{
    [Theory]
    [InlineData("77082336912", true)]
    [InlineData("81072381949", true)]
    [InlineData("00210100019", true)]
    [InlineData("10272300018", true)]
    public void HasValidBirthDate_Returns_True_For_Valid_Pesel(string pesel, bool expected)
    {
        var result = PeselDateValidator.HasValidBirthDate(pesel);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("77992336910", false)]
    [InlineData("77132336910", false)]
    [InlineData("77002336910", false)]
    public void HasValidBirthDate_Returns_False_For_Invalid_Month(string pesel, bool expected)
    {
        var result = PeselDateValidator.HasValidBirthDate(pesel);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("94023136918", false)]
    [InlineData("05033236915", false)]
    public void HasValidBirthDate_Returns_False_For_Invalid_Day(string pesel, bool expected)
    {
        var result = PeselDateValidator.HasValidBirthDate(pesel);

        Assert.Equal(expected, result);
    }
}
