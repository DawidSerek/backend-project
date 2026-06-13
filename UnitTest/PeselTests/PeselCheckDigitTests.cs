using ApplicationCore.Models;
using ApplicationCore.ValueObjects.Pesel;

namespace UnitTest.PeselTests;

public class PeselCheckDigitTests
{
    [Theory]
    [InlineData("7708233691", 2)]
    [InlineData("8107238194", 9)]
    public void CalculateCheckDigit_Returns_Correct_Value(string tenDigits, int expected)
    {
        var result = PeselCheckDigitCalculator.CalculateCheckDigit(tenDigits);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("77082336912", true)]
    [InlineData("12312312312", false)]
    public void IsValid_Returns_Correct_Result(string pesel, bool expected)
    {
        var result = PeselCheckDigitCalculator.IsValid(pesel);

        Assert.Equal(expected, result);
    }
}
