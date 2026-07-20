using DAVE.Models;

namespace DAVE.UnitTests;

public class CheckSiteExclusionReasons
{
    private readonly string _prompt = "Please explain";

    [Fact]
    public void CheckSitesEqual_ShouldPass()
    {
        var check = new CheckSiteExclusions(
            0,
            string.Empty,
            10,
            10,
            _prompt,
            null,
            string.Empty,
            _prompt);

        Assert.True(check.Pass);
    }

    [Fact]
    public void CheckSitesCoveredLessWithNotes_ShouldPass()
    {
        var check = new CheckSiteExclusions(
            0,
            string.Empty,
            9,
            10,
            "Some reason why 9 is less than 10",
            null,
            string.Empty,
            _prompt);

        Assert.True(check.Pass);
    }

    [Fact]
    public void CheckSitesCoveredLessMissingNotes_ShouldFail()
    {
        var check = new CheckSiteExclusions(
            0,
            string.Empty,
            9,
            10,
            string.Empty,
            null,
            string.Empty,
            _prompt);

        Assert.False(check.Pass);
    }

    [Fact]
    public void CheckSitesCoveredLessNotesEqualPrompt_ShouldFail()
    {
        var check = new CheckSiteExclusions(
            0,
            string.Empty,
            9,
            10,
            _prompt,
            null,
            string.Empty,
            _prompt);

        Assert.False(check.Pass);
    }
}
