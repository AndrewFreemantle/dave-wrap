using DAVE.Models;

namespace DAVE.UnitTests;

public class CheckFoodPoMReportedTests
{
    [Fact]
    public void TonnesGiven_UnitsNotGivenNA_ShouldPass()
    {
        var check = new CheckFoodPoMReported(
            0,
            string.Empty,
            10000,
            "N/A",      // allowed values: 'tonnes', 'N/A', blank
            null,
            null,
            string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void TonnesGiven_UnitsNotGivenTonnes_ShouldPass()
    {
        var check = new CheckFoodPoMReported(
            0,
            string.Empty,
            10000,
            "tonnes",      // allowed values: 'tonnes', 'N/A', blank
            null,
            null,
            string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void TonnesGiven_UnitsNotGivenUpperCaseTonnes_ShouldPass()
    {
        var check = new CheckFoodPoMReported(
            0,
            string.Empty,
            10000,
            "TONNES",      // allowed values: 'tonnes', 'N/A', blank
            null,
            null,
            string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void TonnesGiven_UnitsNotGivenBlank_ShouldPass()
    {
        var check = new CheckFoodPoMReported(
            0,
            string.Empty,
            10000,
            string.Empty,      // allowed values: 'tonnes', 'N/A', blank
            null,
            null,
            string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void TonnesNotGiven_UnitsGiven_ShouldPass()
    {
        var check = new CheckFoodPoMReported(
            0,
            string.Empty,
            0,
            "40,000.02 units",
            null,
            null,
            string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void TonnesGiven_UnitsGiven_ShouldFail()
    {
        var check = new CheckFoodPoMReported(
            0,
            string.Empty,
            200000,
            "40,000.02 units",
            null,
            null,
            string.Empty);

        Assert.False(check.Pass);
    }
}
