using ApplicationCore.Services.JaroWinkler;

namespace Tests.UnitTests.JaroWinklerTests;

public class JaroWinklerTests
{
    [Fact]
    public void Test_Same_Same_Strings()
    {
        // Arrange & Act
        var result = JaroWinklerService.Similarity("hello", "hello");

        // Assert
        Assert.Equal(1, Math.Round(result, 2));
    }

    [Fact]
    public void Test_With_Null()
    {
        // Arrange & Act
        var result = JaroWinklerService.Similarity(null, "hellow");

        // Assert
        Assert.Equal(0.0, Math.Round(result, 2));
    }

    [Fact]
    public void Test_Different_Lengths()
    {
        // Arrange & Act
        var result = JaroWinklerService.Similarity("hello", "hellowoooo");

        // Assert
        Assert.Equal(0.9, Math.Round(result, 2));
    }

    [Fact]
    public void Test_Completely_Different_Strings()
    {
        // Arrange & Act
        var result = JaroWinklerService.Similarity("xyz", "abc");

        // Assert
        Assert.Equal(0, Math.Round(result, 2));
    }
}