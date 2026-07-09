using DAVE.Models;

namespace DAVE.UnitTests;

public class CheckDateRangeContinuousTests
{
    [Fact]
    public void FullYear_to_FullYear_ShouldPass()
    {
        var sut = new CheckDateRangeContinuous(1, string.Empty,
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31),
            new DateTime(2024,1,1),
            new DateTime(2024,12,31),
            5,
            string.Empty);

        Assert.True(sut.Pass);
    }

    [Fact]
    public void FullYear_no_PreviousYear_ShouldPass()
    {
        var sut = new CheckDateRangeContinuous(1, string.Empty,
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31),
            null,
            null,
            5,
            string.Empty);

        Assert.True(sut.Pass);
    }

    [Fact]
    public void FullYear_WithGapAtEndOfPrevious_ShouldFail()
    {
        var sut = new CheckDateRangeContinuous(1, string.Empty,
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31),
            new DateTime(2024,1,1),
            new DateTime(2024,12,26),
            5,
            string.Empty);

        Assert.False(sut.Pass);
    }

    [Fact]
    public void FullYear_WithGapAtStart_ShouldFail()
    {
        var sut = new CheckDateRangeContinuous(1, string.Empty,
            new DateTime(2025, 1, 6),
            new DateTime(2025, 12, 31),
            new DateTime(2024,1,1),
            new DateTime(2024,12,31),
            5,
            string.Empty);

        Assert.False(sut.Pass);
    }

    [Fact]
    public void FullYear_to_FullYear_Overlapping_ByAllowed_ShouldPass()
    {
        var sut = new CheckDateRangeContinuous(1, string.Empty,
            new DateTime(2024, 12, 28),
            new DateTime(2025, 12, 31),
            new DateTime(2024,1,1),
            new DateTime(2025,1,1),
            5,
            string.Empty);

        Assert.True(sut.Pass);
    }

    [Fact]
    public void FullYear_to_FullYear_Overlapping_MoreThanAllowed_ShouldFail()
    {
        var sut = new CheckDateRangeContinuous(1, string.Empty,
            new DateTime(2024, 12, 26),
            new DateTime(2025, 12, 31),
            new DateTime(2024,1,1),
            new DateTime(2025,1,1),
            5,
            string.Empty);

        Assert.False(sut.Pass);
    }
}
