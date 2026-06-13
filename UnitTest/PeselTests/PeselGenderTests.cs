using ApplicationCore.Models;
using ApplicationCore.ValueObjects.Pesel;

namespace UnitTest.PeselTests;

public class PeselGenderTests
{
    [Theory]
    [InlineData("77082336911", Gender.Male)]
    [InlineData("77082336912", Gender.Female)]
    public void Extract_Returns_Correct_Gender(string pesel, Gender expected)
    {
        var result = PeselGenderExtractor.Extract(pesel);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Extract_Distinguishes_Male_And_Female()
    {
        var male = PeselGenderExtractor.Extract("77082336911");
        var female = PeselGenderExtractor.Extract("77082336912");

        Assert.NotEqual(male, female);
    }
}
