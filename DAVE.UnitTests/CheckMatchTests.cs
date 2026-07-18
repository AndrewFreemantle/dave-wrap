using DAVE.Models;

namespace DAVE.UnitTests;

public class CheckMatchTests
{
    private readonly string _someString = "This is the text to match";

    [Fact]
    public void CheckMatch_MatchShouldPass()
    {
        var check = new CheckMatch(0, string.Empty, _someString, null, _someString, string.Empty);
        Assert.True(check.Pass);
    }

    [Fact]
    public void CheckMatch_ToggleFlag_MatchShouldFail()
    {
        var check = new CheckMatch(0, string.Empty, _someString, null, _someString, string.Empty, false);
        Assert.False(check.Pass);
    }
}
