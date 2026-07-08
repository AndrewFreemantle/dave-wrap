using DAVE.Models;

namespace DAVE.UnitTests;

public class CheckDateRangeTests
{
    [Fact]
    public void FullYear_ShouldPass()
    {
        var sut = new CheckDateRange(1, string.Empty,
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31),
            0,
            string.Empty);

        Assert.True(sut.Pass);
    }

    [Fact]
    public void DaysAllowedLimit_ShouldPass()
    {
        var daysAllowed = 5;

        var sut = new CheckDateRange(1, string.Empty,
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31).AddDays(0 - daysAllowed),
            daysAllowed,
            string.Empty);

        Assert.True(sut.Pass);
    }

    [Fact]
    public void DaysAllowedLimitPlus1_ShouldFail()
    {
        var daysAllowed = 5;

        var sut = new CheckDateRange(1, string.Empty,
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31).AddDays(0 - (daysAllowed + 1)),
            daysAllowed,
            string.Empty);

        Assert.False(sut.Pass);
    }
}
