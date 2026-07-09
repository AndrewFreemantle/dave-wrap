using DAVE.Models;

namespace DAVE.UnitTests;

public class CheckSiteChangeTests
{
    [Fact]
    public void LessThan10Sites_NoPrevious_ShouldPass()
    {
        var check = new CheckSiteChange(0,
            string.Empty,
            9,
            null,
            string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void LessThan10Sites_NoChange_ShouldPass()
    {
        var check = new CheckSiteChange(0,
            string.Empty,
            8,
            8,
            string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void LessThan10Sites_OneChange_ShouldFail()
    {
        var check = new CheckSiteChange(0,
            string.Empty,
            9,
            8,
            string.Empty);

        Assert.False(check.Pass);
    }

    [Fact]
    public void LessThan30Sites_NoChange_ShouldPass()
    {
        var check = new CheckSiteChange(0,
            string.Empty,
            26,
            26,
            string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void LessThan30Sites_ThreeChanges_ShouldFail()
    {
        var check = new CheckSiteChange(0,
            string.Empty,
            29,
            26,
            string.Empty);

        Assert.False(check.Pass);
    }

    [Fact]
    public void LessThan100Sites_NoChange_ShouldPass()
    {
        var check = new CheckSiteChange(0,
            string.Empty,
            66,
            66,
            string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void LessThan100Sites_SixChanges_ShouldFail()
    {
        var check = new CheckSiteChange(0,
            string.Empty,
            88,
            82,
            string.Empty);

        Assert.False(check.Pass);
    }

    [Fact]
    public void LessThan500Sites_NoChange_ShouldPass()
    {
        var check = new CheckSiteChange(0,
            string.Empty,
            456,
            456,
            string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void LessThan500Sites_NineChanges_ShouldPass()
    {
        var check = new CheckSiteChange(0,
            string.Empty,
            465,
            456,
            string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void LessThan500Sites_ElevenChanges_ShouldFail()
    {
        var check = new CheckSiteChange(0,
            string.Empty,
            490,
            479,
            string.Empty);

        Assert.False(check.Pass);
    }

    [Fact]
    public void MoreThan1000Sites_49Changes_ShouldPass()
    {
        var check = new CheckSiteChange(0,
            string.Empty,
            2049,
            2000,
            string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void MoreThan1000Sites_50Changes_ShouldFail()
    {
        var check = new CheckSiteChange(0,
            string.Empty,
            3050,
            3000,
            string.Empty);

        Assert.False(check.Pass);
    }
}
