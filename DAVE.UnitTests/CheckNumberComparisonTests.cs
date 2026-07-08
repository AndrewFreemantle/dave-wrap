using DAVE.Models;

namespace DAVE.UnitTests;

public class CheckNumberComparisonTests
{
    [Fact]
    public void LessThanTwentyPercentDifference_ShouldPass()
    {
        var sut = new CheckNumberComparison(1, string.Empty,
            100000,
            110000,
            20,
            string.Empty);

        Assert.True(sut.Pass);
    }

    [Fact]
    public void LessThanTwentyPercentDifferenceValuesSwapped_ShouldPass()
    {
        var sut = new CheckNumberComparison(1, string.Empty,
            110000,
            100000,
            20,
            string.Empty);

        Assert.True(sut.Pass);
    }

    [Fact]
    public void NothingToCompareAgainst_ShouldPass()
    {
        var sut = new CheckNumberComparison(1, string.Empty,
            100000,
            null,
            20,
            string.Empty);

        Assert.True(sut.Pass);
    }

    [Fact]
    public void MoreThanTwentyPercentDifference_ShouldFail()
    {
        var sut = new CheckNumberComparison(1, string.Empty,
            100000,
            120001,
            20,
            string.Empty);

        Assert.False(sut.Pass);
    }

}
