using DAVE.Models;

namespace DAVE.UnitTests;

public class CheckIfGivenTests
{
    [Fact]
    public void CheckIfGiven_Given_ShouldPass()
    {
        var check = new CheckIfGiven(0, string.Empty, "Some Value", null, string.Empty);

        Assert.True(check.Pass);
    }

    [Fact]
    public void CheckIfGiven_NotGiven_ShouldFail()
    {
        var check = new CheckIfGiven(0, string.Empty, string.Empty, null, string.Empty);

        Assert.False(check.Pass);
    }

    [Fact]
    public void CheckIfGiven_GivenInList_ShouldPass()
    {
        var check = new CheckIfGiven(0, string.Empty, "Some Value", null, string.Empty, ["Some Value"]);

        Assert.True(check.Pass);
    }
}
