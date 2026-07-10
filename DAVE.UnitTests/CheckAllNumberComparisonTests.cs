using DAVE.Models;

namespace DAVE.UnitTests;

public class CheckAllNumberComparisonTests
{
    [Fact]
    public void CompareZeroToZero_ShouldPass()
    {
        var left = new List<decimal> { 0m };
        var right = new List<decimal> { 0m };

        var check = new CheckAllNumberComparison(
            0,
            string.Empty,
            left,
            right,
            10,
            string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void CompareMultiplePairs_ShouldPass()
    {
        var left = new List<decimal> { 0m, 100m, 200m, 500m, 1000m };
        var right = new List<decimal> { 0m, 102m, 212m, 549m, 1099m };

        var check = new CheckAllNumberComparison(
            0,
            string.Empty,
            left,
            right,
            10,
            string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void CompareZeroToAnything_ShouldFail()
    {
        var left = new List<decimal> { 0m };
        var right = new List<decimal> { 0.5m };

        var check = new CheckAllNumberComparison(
            0,
            string.Empty,
            left,
            right,
            10,
            string.Empty);

        Assert.False(check.Pass);
    }

    [Fact]
    public void CompareAnythingToZero_ShouldFail()
    {
        var left = new List<decimal> { 1m };
        var right = new List<decimal> { 0m };

        var check = new CheckAllNumberComparison(
            0,
            string.Empty,
            left,
            right,
            10,
            string.Empty);

        Assert.False(check.Pass);
    }
}
